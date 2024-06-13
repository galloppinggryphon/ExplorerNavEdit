using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace ExplorerNav.Models
{
    internal class NavItemList : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
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
                OnPropertyChanged(nameof(HasItems));
            }
        }

        public bool HasItems
        {
            get => Items.Count > 0;
        }

        public NavItemList()
        {
            Items = new();
        }

        public void Add(NavItem item, bool insertFirst = false)
        {
            if (insertFirst)
            {
                Items.Insert(0, item);
            }
            else
            {
                Items.Add(item);
            }

            OnPropertyChanged(nameof(HasItems));
        }

        public void Add(NavItem.ItemData itemData)
        {
            //Items.Prepend
            Items.Add(new NavItem(itemData));
            OnPropertyChanged(nameof(HasItems));
        }

        public void Remove(NavItem item)
        {
            Items.Remove(item);
            OnPropertyChanged(nameof(HasItems));
        }

        public void RemoveAll()
        {
            Items.Clear();
        }

        public List<NavItem.ItemData> Export()
        {
            return Items.Select(item => item.Export()).ToList();
        }
    }
}
