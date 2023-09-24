using Microsoft.Win32;

namespace ExplorerNav.Models
{
    internal class RegistryLocation
    {
        public RegistryKey? Key { get; set; }
        public RegistryKey? PrevKey { get; set; }
        public string Path { get; private set; } = "";
        public bool IsValid { get; private set; }
        public bool IsWriteable { get; set; }

        public int SubKeyCount
        {
            get => Key.SubKeyCount;
        }
        public string[] SubKeyNames
        {
            get => Key.GetSubKeyNames();
        }

        static public RegistryLocation CurrentUser(string subKey, bool writeable = false)
        {
            var key = new RegistryLocation(Registry.CurrentUser, writeable);
            key.OpenSubKey(subKey);
            return key;
        }

        static public RegistryLocation LocalMachine(string subKey, bool writeable = false)
        {
            var key = new RegistryLocation(Registry.LocalMachine, writeable);
            key.OpenSubKey(subKey);
            return key;
        }

        public RegistryLocation(bool writable = false)
        {
            IsWriteable = writable;
        }

        public RegistryLocation(RegistryKey key)
        {
            if (key != null)
            {
                IsValid = true;
                Key = key;
                SetPath(key.ToString());
            }
        }

        public RegistryLocation(RegistryKey key, bool writable = false)
        {
            if (key != null)
            {
                IsValid = true;
                IsWriteable = writable;
                Key = key;
                SetPath(key.ToString());
            }
        }

        public RegistryLocation(string key, bool writable = false)
        {
            OpenSubKey(key, writable);
        }

        public RegistryLocation(RegistryKey prevKey, string subKey, bool writable)
        {
            if (prevKey != null)
            {
                OpenSubKey(prevKey, subKey, writable);
            }
        }
        static public RegistryLocation Open(string key, bool isWriteable = false)
        {
            return new RegistryLocation(key, isWriteable);
        }

        public RegistryLocation GetSubKey(string subKey, bool isWritable = false)
        {
            return new RegistryLocation(Key, subKey, isWritable);
        }

        public RegistryLocation OpenSubKey(string subKey, bool? isWritable = null)
        {
            return OpenSubKey(Key, subKey, isWritable);
        }

        public RegistryLocation OpenSubKey(RegistryKey prevKey, string subKey, bool? isWritable = null)
        {
            bool writable = isWritable ?? IsWriteable;
            PrevKey = prevKey;
            Key = prevKey.OpenSubKey(subKey, writable);

            if (Key == null)
            {
                IsValid = false;
                SetPath(prevKey.ToString(), subKey);
            }
            else
            {
                IsValid = true;
                SetPath(Key.ToString());
            }

            return this;
        }

        public RegistryLocation NewKey(string keyName, bool setCurrentKey = false)
        {
            var newKey = Key.CreateSubKey(keyName);
            if (setCurrentKey)
            {
                if (newKey != null)
                {
                    IsValid = true;
                }
                Key = newKey;
                return this;
            }
            return new RegistryLocation(newKey);
        }

        public RegistryLocation DeleteTree(string keyName)
        {
            Key.DeleteSubKeyTree(keyName, false);
            return this;
        }

        public RegistryLocation DeleteKey(string keyName)
        {
            Key.DeleteSubKey(keyName, false);
            return this;
        }

        public RegistryLocation DeleteValue(string valueKey)
        {
            Key.DeleteValue(valueKey, false);
            return this;
        }

        public string? GetString(string keyName = "")
        {
            var value = Key.GetValue(keyName);
            return value == null ? null : (string)value;
        }

        public int? GetInt(string keyName = "")
        {
            var value = Key.GetValue(keyName);
            return value == null ? null : (int)value;
        }

        public RegistryLocation WriteString(string keyName, string value)
        {
            Key.SetValue(keyName, value);
            return this;
        }

        public RegistryLocation WriteExpandString(string keyName, string value)
        {
            Key.SetValue(keyName, value, RegistryValueKind.ExpandString);
            return this;
        }

        public RegistryLocation WriteDword(string keyName, uint value)
        {
            //Undocumented bug
            //Dword is an unsigned int, but SetValue converts value back to 32bit signed int
            //https://stackoverflow.com/a/60678875/8785542
            Key.SetValue(keyName, unchecked((int)value), RegistryValueKind.DWord);
            return this;
        }

        private string SetPath(string path, string key = "")
        {
            Path = System.IO.Path.Join(path, key);
            return Path;
        }
    }
}
