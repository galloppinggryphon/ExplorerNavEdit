using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ExplorerNav.Models
{
    interface INavItem
    {
        public string Uid { get; set; }
        public string Title { get; set; }
        public string Path { get; set; }
        public string Icon { get; set; }
    }

    internal enum NavItemKeys
    {
        Uid,
        Title,
        Path,
        Icon,
        IsBuiltIn,
    }

    internal class NavItem : INavItem, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        static private readonly NavItemKeys[] _keys = { NavItemKeys.Uid, NavItemKeys.Title, NavItemKeys.Icon, NavItemKeys.Path };

        private string _title;
        private string _uid;
        private string _path;
        private string _icon;

        public NavItemState State { get; } = new(_keys);

        public string Title
        {
            get => _title;
            set
            {
                if (State.Fields.ShouldUpdateValue(NavItemKeys.Title, _title, value))
                {
                    _title = value;
                    OnPropertyChanged(nameof(Title));
                    State.UpdateSaveStatus();
                }
            }
        }

        public string Uid
        {
            get => _uid;
            set
            {
                if (State.Fields.ShouldUpdateValue(NavItemKeys.Uid, _uid, value))
                {
                    _uid = value;
                    OnPropertyChanged(nameof(Uid));
                    State.UpdateSaveStatus();
                }
            }
        }

        public string Path
        {
            get => _path;
            set
            {
                if (State.Fields.ShouldUpdateValue(NavItemKeys.Path, _path, value))
                {
                    _path = value;
                    OnPropertyChanged(nameof(Path));
                    State.UpdateSaveStatus();
                }
            }
        }

        public string Icon
        {
            get => _icon;
            set
            {
                if (State.Fields.ShouldUpdateValue(NavItemKeys.Icon, _icon, value))
                {
                    _icon = value;
                    OnPropertyChanged(nameof(Icon));
                    State.UpdateSaveStatus();
                }
            }
        }

        public NavItem()
        {
            NewUID();
            State.Fields.Start();
            State.RefreshStatus();
        }

        public NavItem(string title)
        {
            NewUID();
            Title = title;
            State.SetApplied(false);
            State.Fields.Refresh(ValuesAsArray());
            State.Fields.Start();
            State.RefreshStatus();
        }

        public NavItem(string uid, string title, string path, string icon, bool isBuiltIn = false, bool isApplied = false)
        {
            Title = title;
            Path = path;
            Icon = icon;
            Uid = uid;

            State.SetType(isBuiltIn);
            State.SetApplied(isApplied);

            State.Fields.Refresh(ValuesAsArray());
            State.Fields.Start();

            State.RefreshStatus();
        }

        public void NewUID()
        {
            Uid = Guid.NewGuid().ToString(); //"THIS_IS_A_TEST_UID";
        }

        public void SetUID(string uid)
        {
            Uid = uid;
        }

        public void SetTitle(string title)
        {
            Title = title;
        }

        public void SetPath(string path)
        {
            Path = path;
        }

        public void SetIcon(string icon)
        {
            Icon = icon;
        }


        //public void SetIsApplied(bool value)
        //{
        //    State.SetApplied(value);
        //}

        //public void SetIsSaved(bool value)
        //{
        //    State.SetApplied(value);
        //}

        public void OnSaved()
        {
            State.Fields.Refresh(ValuesAsArray());

            //OnPropertyChanged(nameof(SaveStatusText));
            //OnPropertyChanged(nameof(ApplyStatusText));

            //UpdateSaveTextStatus();
            //UpdateApplyTextStatus();
        }

        public List<string>? Validate()
        {
            List<string> errors = new();

            if (Title == "" || Title == null)
            {
                errors.Add("title");
            }

            if (Path == "" || Path == null)
            {
                errors.Add("path");
            }

            if (Icon == "" || Icon == null)
            {
                errors.Add("icon");
            }

            return errors.Count == 0 ? null : errors;
        }

        public string[] ValuesAsArray()
        {
            return new string[] { Uid, Title, Icon, Path };
        }
    }
}
