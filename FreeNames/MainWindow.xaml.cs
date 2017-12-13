using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Xml;
using System.Xml.Linq;

namespace FreeNames
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<string> _freeNames;

        private ObservableCollection<string> _freeAddresses;

        public MainWindow()
        {
            InitializeComponent();

            ReadSettingsFromXmlFile("Settings.xml");
        }

        private async void btNamesSearch_Click(object sender, RoutedEventArgs e)
        {
            btNamesSearch.IsEnabled = false;

            _freeNames = new ObservableCollection<string>();

            lbFreeNames.ItemsSource = _freeNames;

            var searchParams = ValidateNameSearchParams();

            if (searchParams != null)
            {
                var finder = new Finder();

                try
                {
                    await finder.SearchFreeNamesAsync(searchParams);
                }
                catch(Exception ex)
                {
                    ShowErrorMessage(GetExceptionDetails(ex));
                }
            }

            btNamesSearch.IsEnabled = true;
        }

        private async void btAddressesSearch_Click(object sender, RoutedEventArgs e)
        {
            btAddressesSearch.IsEnabled = false;

            _freeAddresses = new ObservableCollection<string>();

            lbFreeAddresses.ItemsSource = _freeAddresses;

            var searchParams = ValidateAddressesSearchParams();

            if (searchParams != null)
            {
                var finder = new Finder();

                try
                {
                    await finder.SearchFreeAddressesAsync(searchParams);
                }
                catch (Exception ex)
                {
                    ShowErrorMessage(GetExceptionDetails(ex));
                }
            }

            btAddressesSearch.IsEnabled = true;
        }

        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private string GetExceptionDetails(Exception ex)
        {
            return ex.GetType() + "\n" + ex.Message;
        }

        private void AddFreeName(string name)
        {
            _freeNames.Add(name);
        }

        private void AddFreeAddress(string address)
        {
            _freeAddresses.Add(address);
        }

        /// <summary>
        /// Валидация параметров поиска свободных имен.
        /// </summary>
        /// <returns>
        /// Объект SearchFreeNamesParams - при успешной валидации.
        /// Null - при ошибке валидации.
        /// </returns>
        private SearchFreeNamesParams ValidateNameSearchParams()
        {
            string nameTemplate = tbNameTemplate.Text;

            if (String.IsNullOrWhiteSpace(nameTemplate))
            {
                ShowErrorMessage("Укажите шаблон имени.");
                return null;
            }

            if (!nameTemplate.Contains("[C]"))
            {
                ShowErrorMessage("Добавьте счетчик [C] в шаблон имени.");
                return null;
            }

            string domain = tbDomain.Text;

            if (String.IsNullOrWhiteSpace(domain))
            {
                ShowErrorMessage("Укажите домен.");
                return null;
            }

            uint counterFrom, counterTo, counterLength;

            try
            {
                counterFrom = UInt32.Parse(tbCounterFrom.Text);
                counterTo = UInt32.Parse(tbСounterTo.Text);
                counterLength = UInt32.Parse(tbCounterLength.Text);
            }
            catch
            {
                ShowErrorMessage("Укажите параметры счетчика - целые неотрицательные числа.");
                return null;
            }

            return new SearchFreeNamesParams()
            {
                NameTemplate = nameTemplate,
                CounterFrom = counterFrom,
                CounterTo = counterTo,
                CounterLength = counterLength,
                Domain = domain,
                Dispatcher = Dispatcher,
                Callback = AddFreeName
            };
        }

        /// <summary>
        /// Валидация параметров поиска свободных адресов.
        /// </summary>
        /// <returns>
        /// Объект SearchFreeAddressesParams - при успешной валидации.
        /// Null - при ошибке валидации.
        /// </returns>
        private SearchFreeAddressesParams ValidateAddressesSearchParams()
        {
            if (String.IsNullOrWhiteSpace(tbSubnets.Text))
            {
                ShowErrorMessage("Укажите хотя бы одну подсеть.");
                return null;
            }

            var searchParams = new SearchFreeAddressesParams()
            {
                Subnets = new List<string>(),
                Dispatcher = Dispatcher,
                Callback = AddFreeAddress
            };

            searchParams.Subnets.AddRange(tbSubnets.Text.Replace("\r", "").Split('\n'));

            return searchParams;
        }

        private void ReadSettingsFromXmlFile(string fileName)
        {
            XDocument doc = XDocument.Load(fileName);

            var nameTemplate = doc.Root.Elements("NameTemplate").SingleOrDefault();
            var counterFrom = doc.Root.Elements("CounterFrom").SingleOrDefault();
            var counterTo = doc.Root.Elements("CounterTo").SingleOrDefault();
            var counterLength = doc.Root.Elements("CounterLength").SingleOrDefault();
            var domain = doc.Root.Elements("Domain").SingleOrDefault();
            var subnets = doc.Root.Elements("Subnet");

            if (nameTemplate != null)
                tbNameTemplate.Text = nameTemplate.Value;

            if (counterFrom != null)
                tbCounterFrom.Text = counterFrom.Value;

            if (counterTo != null)
                tbСounterTo.Text = counterTo.Value;

            if (counterLength != null)
                tbCounterLength.Text = counterLength.Value;

            if (domain != null)
                tbDomain.Text = domain.Value;

            if (subnets != null)
                foreach (var subnet in subnets)
                    tbSubnets.Text += subnet.Value + "\n";
        }
    }
}
