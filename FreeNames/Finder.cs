using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace FreeNames
{
    /// <summary>
    /// Класс для поиска свободных имен и адресов.
    /// </summary>
    public class Finder
    {
        /// <summary>
        /// Поиск свободных имен.
        /// </summary>
        public void SearchFreeNames(SearchFreeNamesParams searchParams)
        {
            for (var i = searchParams.CounterFrom; i <= searchParams.CounterTo; i++)
            {
                var name = searchParams.NameTemplate.Replace("[C]", i.ToString().PadLeft((int)searchParams.CounterLength, '0'));

                if (ExistsInDNS(name + "." + searchParams.Domain))
                    continue;

                if (ExistsInAD(name, searchParams.Domain))
                    continue;

                searchParams.Dispatcher.BeginInvoke(searchParams.Callback, name);
            }
        }

        /// <summary>
        /// Асинхронный поиск свободных имен.
        /// </summary>
        public Task SearchFreeNamesAsync(SearchFreeNamesParams searchParams) 
        {
            return Task.Factory.StartNew(() => SearchFreeNames(searchParams));
        }

        /// <summary>
        /// Поиск свободных адресов.
        /// </summary>
        public void SearchFreeAddresses(SearchFreeAddressesParams searchParams)
        {
            foreach (var subnet in searchParams.Subnets)
            {
                if (String.IsNullOrWhiteSpace(subnet))
                    continue;

                for (int i = 1; i < 255; i++)
                {
                    string address = subnet + "." + i.ToString();

                    if (!ValidateIPAddress(address))
                        continue;

                    if (ExistsInDNS(address))
                        continue;

                    if (RepliesToPing(address))
                        continue;

                    searchParams.Dispatcher.BeginInvoke(searchParams.Callback, address);
                }
            }
        }

        /// <summary>
        /// Асинхронный поиск свободных адресов.
        /// </summary>
        public Task SearchFreeAddressesAsync(SearchFreeAddressesParams searchParams)
        {
            return Task.Factory.StartNew(() => SearchFreeAddresses(searchParams));
        }

        /// <summary>
        /// Проверяет, есть ли DNS-запись для указанного имени хоста или IP-адреса.
        /// </summary>
        private bool ExistsInDNS(string hostNameOrAddress)
        {
            try
            {
                Dns.GetHostEntry(hostNameOrAddress);

                return true;
            }
            catch (SocketException ex)
            {
                if (ex.ErrorCode == 11001)
                    return false;

                throw;
            }
        }

        ///<summary>
        /// Проверяет, есть ли учетная запись Active Directory для указанного имени компьютера в указанном домене.
        /// </summary>
        private bool ExistsInAD(string computerName, string domain)
        {
            PrincipalContext context = new PrincipalContext(ContextType.Domain, domain);

            if (ComputerPrincipal.FindByIdentity(context, computerName) != null)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Проверяет, является ли указанная строка IP-адресом.
        /// </summary>
        private bool ValidateIPAddress(string address)
        {
            if (String.IsNullOrWhiteSpace(address))
                return false;

            string[] splitValues = address.Split('.');

            if (splitValues.Length != 4)
                return false;

            byte tempForParsing;

            return splitValues.All(r => byte.TryParse(r, out tempForParsing));
        }

        /// <summary>
        /// Проверяет, отвечает ли указанный адрес на пинги.
        /// </summary>
        private bool RepliesToPing(string address)
        {
            var ping = new Ping();

            for (var i = 0; i < 2; i++)
            {
                if (ping.Send(address).Status == IPStatus.Success)
                    return true;
            }

            return false;
        }
    }
}
