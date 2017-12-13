using System.Windows.Threading;

namespace FreeNames
{
    /// <summary>
    /// Класс для передачи параметров поиска свободных имен.
    /// </summary>
    public class SearchFreeNamesParams
    {
        public string NameTemplate;

        public uint CounterFrom;

        public uint CounterTo;

        public uint CounterLength;

        public string Domain;

        /// <summary>
        /// Диспетчер окна WPF. Нужен для обращения к элементам окна из другого потока.
        /// </summary>
        public Dispatcher Dispatcher;

        /// <summary>
        /// Этот метод вызывается когда найдено свободное имя.
        /// </summary>
        public NewItemFoundDelegate Callback;
    }
}
