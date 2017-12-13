using System.Collections.Generic;
using System.Windows.Threading;

namespace FreeNames
{
    /// <summary>
    /// Класс для передачи параметров поиска свободных адресов.
    /// </summary>
    public class SearchFreeAddressesParams
    {
        public List<string> Subnets;

        /// <summary>
        /// Диспетчер окна WPF. Нужен для обращения к элементам окна из другого потока.
        /// </summary>
        public Dispatcher Dispatcher;

        /// <summary>
        /// Этот метод вызывается когда найден свободный адрес.
        /// </summary>
        public NewItemFoundDelegate Callback;
    }
}
