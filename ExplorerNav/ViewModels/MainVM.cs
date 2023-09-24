using ExplorerNav.Models;
using ExplorerNav.Services;
using System.ComponentModel;

namespace ExplorerNav.ViewModels
{
    internal class MainVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private readonly DialogueService dialogueService = new();
        private readonly NavEdit editor = new();

        private NavItemList _navList = new();
        private NavItem _currentItem;
        private bool _isEditorEnabled = false;

        public bool IsEditorEnabled
        {
            get => _isEditorEnabled;
            set
            {
                _isEditorEnabled = value;
                OnPropertyChanged(nameof(IsEditorEnabled));
            }
        }

        public NavItemList NavList
        {
            get => _navList;
            set
            {
                _navList = value;
                OnPropertyChanged(nameof(NavList));
            }
        }

        public NavItem CurrentItem
        {
            get => _currentItem;
            set
            {
                _currentItem = value;
                OnPropertyChanged(nameof(CurrentItem));
                IsEditorEnabled = value != null;
            }
        }

        public MainVM()
        {
            NavList = new();
        }

        public void ReadNavItemsFromRegistry()
        {
            IsEditorEnabled = false;
            NavList.Items = editor.ReadNavItemsFromRegistry();
        }

        public void RegisterNavItem()
        {
            var errors = CurrentItem.Validate();

            if (errors == null)
            {
                editor.WriteNavItemToRegistry(CurrentItem);

                CurrentItem.State.SetApplied(true);
                CurrentItem.State.SetSaved(true);

                CurrentItem.OnSaved();

                dialogueService.ShowInfo("Applied to registry", "The navigation item has been saved to the registry.\n\nYou may have to stop and restart explorer.exe from the task manager before it will work.");
            }
            else
            {
                string errorStr = string.Join(", ", errors.ToArray());
                dialogueService.ShowError("Item not applied", "Not able to register this item.\n\nThe following fields are empty/invalid: " + errorStr);
            }
        }

        public void UnregisterNavItem()
        {
            if (CurrentItem.State.IsApplied)
            {
                if (!dialogueService.AskYesNo("Unregister item?", $"Are you sure you want to remove '{CurrentItem.Title}' from the Windows registry?")) return;

                if (CurrentItem.State.IsBuiltIn && !dialogueService.AskYesNo("Unregister built-in item?", $"'{CurrentItem.Title}' is a Windows built-in item!\n\n Are you absolutely sure you want to remove it?", true)) return;

                editor.RemoveNavItemFromRegistry(CurrentItem);
                CurrentItem.State.SetApplied(false);
            }
        }

        public void NewItem()
        {
            CurrentItem = new NavItem("[New Item]");
            SetCurrentItemIcon();
            CurrentItem.State.SetSaved(true);
            CurrentItem.State.SetApplied(false);
            NavList.Add(CurrentItem);
        }

        public void RemoveCurrentItem()
        {
            NavList.Remove(CurrentItem);
        }

        public void SetCurrentItemIcon()
        {
            CurrentItem.SetIcon(IconPicker.DefaultIconfile + ",0");
        }

        public void SetCurrentItemIcon(IconPicker.IconData icon)
        {
            CurrentItem.SetIcon(icon.ToString());
        }

        public void EditorEnable()
        {
            IsEditorEnabled = true;
        }

        public void ShowBrowseDirectory()
        {
            string? path = dialogueService.ShowDirectoryBrowse(CurrentItem.Path);

            if (path != null)
            {
                CurrentItem.Path = path;
            }
        }

        public void ShowAboutWindow()
        {
            dialogueService.ShowInfo("About Explorer Navigation Editor", "Explorer Navigation Editor\n(C) Bjornar Egede-Nissen, 2023\nLicense: MIT");
        }
    }
}
