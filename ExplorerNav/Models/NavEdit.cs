using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ExplorerNav.Models
{
    internal class NavEdit
    {
        public Dictionary<string, string> Errors { get; private set; } = new();

        public void WriteNavItemToRegistry(NavItem item)
        {
            string uid = "{" + item.Uid + "}";

            var keyClsid = RegistryLocation.CurrentUser(@"Software\Classes\CLSID\", true);
            var keyClsid64 = RegistryLocation.CurrentUser(@"Software\Classes\Wow6432Node\CLSID", true);

            keyClsid.NewKey(uid, true)
                .WriteString("", item.Title)
                .WriteDword(@"System.IsPinnedToNamespaceTree", 0x1)
                .WriteDword("SortOrderIndex", 0x42);

            keyClsid64.NewKey(uid, true)
                .WriteString("", item.Title)
                .WriteDword(@"System.IsPinnedToNamespaceTree", 0x1)
                .WriteDword("SortOrderIndex", 0x42);

            keyClsid
                .NewKey("InProcServer32")
                .WriteExpandString("", @"%SYSTEMROOT%\system32\shell32.dll");

            keyClsid64
                .NewKey("InProcServer32")
                .WriteExpandString("", @"%SYSTEMROOT%\system32\shell32.dll");

            keyClsid
                .NewKey("ShellFolder")
                .WriteDword("FolderValueFlags", 0x28)
                .WriteDword("Attributes", 0xf080004d);

            keyClsid64
                .NewKey("ShellFolder")
                .WriteDword("FolderValueFlags", 0x28)
                .WriteDword("Attributes", 0xf080004d);

            keyClsid
                .NewKey("DefaultIcon")
                .WriteExpandString("", item.Icon);

            keyClsid64
                .NewKey("DefaultIcon")
                .WriteExpandString("", item.Icon);

            keyClsid
                .NewKey("Instance")
                .WriteString("CLSID", "{0E5AAE11-A475-4c5b-AB00-C66DE400274E}")
                .NewKey("InitPropertyBag")
                .WriteDword("Attributes", 0x11)
                .WriteExpandString("TargetFolderPath", item.Path);

            keyClsid64
                .NewKey("Instance")
                .WriteString("CLSID", "{0E5AAE11-A475-4c5b-AB00-C66DE400274E}")
                .NewKey("InitPropertyBag")
                .WriteDword("Attributes", 0x11)
                .WriteExpandString("TargetFolderPath", item.Path);

            //SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\HideDesktopIcons\NewStartPanel
            RegistryLocation
                .CurrentUser(@"Software\Microsoft\Windows\CurrentVersion\Explorer\HideDesktopIcons\NewStartPanel", true)
                .WriteDword(uid, 0x1);

            //HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Desktop\NameSpace
            RegistryLocation
                .CurrentUser(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Desktop\NameSpace", true)
                .NewKey(uid)
                .WriteString("", item.Title);
        }

        public ObservableCollection<NavItem> ReadNavItemsFromRegistry()
        {
            //Get list from Computer\HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Desktop\NameSpace
            NavItemList navList = new();

            var desktopNamespace = RegistryLocation.CurrentUser(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Desktop\NameSpace");

            //HKEY_CURRENT_USER\Software\Classes\Wow6432Node\CLSID
            var regKeyClsid = RegistryLocation.CurrentUser(@"Software\Classes\Wow6432Node\CLSID", false);

            string[] keys = new string[] { };
            if (desktopNamespace.SubKeyCount > 0)
            {
                keys = desktopNamespace.SubKeyNames;
            }

            int i = 0;


            foreach (string key in keys)
            {
                //if (i == 1) continue;
                i++;

                string uid = key.Substring(1, key.Length - 2); //key.IndexOf('}')

                string? error = null;

                string title = "",
                    icon = "",
                    folder = "";

                bool isBuiltIn = false;

                try
                {
                    title = desktopNamespace.GetSubKey(key).GetString();
                    var current = regKeyClsid.GetSubKey(key);
                    icon = current.GetSubKey("DefaultIcon").GetString();
                    current.OpenSubKey(@"Instance\InitPropertyBag");

                    string knownFolder = current.GetString("TargetKnownFolder") ?? "";
                    string folderPath = current.GetString("TargetFolderPath") ?? "";

                    isBuiltIn = knownFolder != "";
                    folder = isBuiltIn ? knownFolder : folderPath;
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                }
                finally
                {
                    NavItem item = new NavItem(uid, title.ToString(), folder.ToString(), icon.ToString(), isBuiltIn, true);

                    if (error != null)
                    {
                        item.State.ErrorStatus = ErrorEnum.ReadError;
                        item.State.ErrorMessage = error;
                    }

                    item.State.Fields.Start();
                    navList.Add(item);
                }
            }

            return navList.Items;
        }

        public void RemoveNavItemFromRegistry(NavItem item)
        {
            string uid = "{" + item.Uid + "}";

            try
            {
                RegistryLocation.CurrentUser(@"Software\Classes\CLSID", true).DeleteTree(uid);
                RegistryLocation.CurrentUser(@"Software\Classes\Wow6432Node\CLSID", true).DeleteTree(uid);
                RegistryLocation.CurrentUser(@"Software\Microsoft\Windows\CurrentVersion\Explorer\HideDesktopIcons\NewStartPanel", true).DeleteValue(uid);
                RegistryLocation.CurrentUser(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Desktop\NameSpace", true).DeleteKey(uid);
            }
            catch { }
        }
    }
}
