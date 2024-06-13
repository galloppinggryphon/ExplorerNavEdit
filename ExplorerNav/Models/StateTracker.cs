using System.Collections.Generic;
using System.ComponentModel;

namespace ExplorerNav.Models
{
    class StateTracker<Keys> : INotifyPropertyChanged where Keys : notnull
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private readonly Dictionary<Keys, string> _originalValues = new();
        private HashSet<Keys> _dirtyKeys = new();
        private bool _someKeysDirty;

        private bool Tracking { get; set; }

        public HashSet<Keys> DirtyKeys
        {
            get => _dirtyKeys;
            private set
            {
                _dirtyKeys = value;
                SomeKeysDirty = _dirtyKeys.Count > 0;
                OnPropertyChanged(nameof(DirtyKeys));
            }
        }

        public bool SomeKeysDirty
        {
            get => _someKeysDirty;
            private set
            {
                _someKeysDirty = value;
                OnPropertyChanged(nameof(SomeKeysDirty));
            }
        }

        public StateTracker(Keys[] keys)
        {
            string[] arr = new string[keys.Length];

            for (int i = 0; i < keys.Length; i++)
            {
                arr[i] = "";
            }

            SetItems(keys, arr);
        }

        public StateTracker(Keys[] keys, string[] values)
        {
            SetItems(keys, values);
        }

        private void SetItems(Keys[] keys, string[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                Keys key = keys[i];
                _originalValues[key] = values[i];
            }
        }

        public void Start()
        {
            Tracking = true;
        }

        public void Stop()
        {
            Tracking = false;
        }

        public void Refresh(string[] values)
        {
            DirtyKeys.Clear();

            int i = 0;
            foreach (var item in _originalValues)
            {
                _originalValues[item.Key] = values[i];
                i++;
            }
        }

        public bool ShouldUpdateValue(Keys key, string prevValue, string value)
        {
            if (!Tracking) return true;

            bool isOriginal = _originalValues[key] == value;
            bool hasChanged = (value == "" || value == null)
                    || prevValue != value
                   || !isOriginal;

            if (isOriginal)
            {
                SetKeyNotDirty(key);
            }
            else if (hasChanged)
            {
                SetKeyDirty(key);
            }

            return hasChanged;
        }

        public bool HasValueChanged(Keys key, string prevValue, string value)
        {
            if (!Tracking) return true;

            bool isOriginal = _originalValues[key] == value;
            bool hasChanged = (value == "" || value == null)
                    || prevValue != value
                   || !isOriginal;

            if (isOriginal)
            {
                SetKeyNotDirty(key);
            }
            else if (hasChanged)
            {
                SetKeyDirty(key);
            }

            return hasChanged;
        }


        private void SetKeyDirty(Keys key)
        {
            DirtyKeys.Add(key);
            OnPropertyChanged(nameof(DirtyKeys));
            SomeKeysDirty = _dirtyKeys.Count > 0;
        }

        private void SetKeyNotDirty(Keys key)
        {

            DirtyKeys.Remove(key);
            OnPropertyChanged(nameof(DirtyKeys));
            SomeKeysDirty = _dirtyKeys.Count > 0;
        }
    }

}
