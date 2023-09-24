using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ExplorerNav.Models
{
    internal class NavItemList : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private ObservableCollection<NavItem> _items = new();

        public ObservableCollection<NavItem> Items
        {
            get
            {
                return _items;
            }
            set
            {
                _items = value;
                this.OnPropertyChanged(nameof(Items));
            }
        }

        public NavItemList()
        {
            Items = new();
        }

        public void Add(NavItem item)
        {
            Items.Add(item);
        }

        public void Remove(NavItem item)
        {
            Items.Remove(item);
        }
    }
}
