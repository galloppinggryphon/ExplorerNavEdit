using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace ExplorerNav.Services
{
    internal class IconPicker
    {
        internal class IconData
        {
            public int Index { get; set; } = 0;
            public string? Path { get; set; }

            public IconData() { }

            public IconData(string iconString)
            {
                string[] strings = iconString.Split(',');

                if (strings.Length > 0)
                {
                    Path = strings[0];

                    if (strings.Length == 2)
                    {
                        Index = int.Parse(strings[1]);
                    }
                    else if (strings.Length > 2)
                    {
                        throw new ArgumentException("Invalid icon string: " + iconString);
                    }
                }
            }

            public IconData(int index, string? path)
            {
                Index = index;
                Path = path;
            }

            public override string ToString()
            {
                return Path == null ? "" : Path + ',' + Index;
            }

            //// convert icon handle to ImageSource
            //this.myImage.Source = Imaging.CreateBitmapSourceFromHIcon(largeIcons[0],
            //    Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }

        public static readonly string DefaultIconfile = Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\shell32.dll";

        private Window _window;

        private const int MAX_PATH = 0x00000104;

        //From https://stackoverflow.com/a/55997245/8785542
        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        private static extern int PickIconDlg(IntPtr hwndOwner, System.Text.StringBuilder lpstrFile, int nMaxFile, ref int lpdwIconIndex);

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        private static extern uint ExtractIconEx(string szFileName, int nIconIndex, IntPtr[] phiconLarge, IntPtr[] phiconSmall, uint nIcons);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool DestroyIcon(IntPtr handle);

        public static IconData ParseIconString(string iconString)
        {
            return new IconData(iconString);
        }

        public IconData CurrentIcon { get; set; } = new();

        public IconPicker(Window window)
        {
            //Bind window that will spawn the dialogue
            _window = window;
        }

        public IconData Icon()
        {
            return new IconData();
        }

        private IconData SetCurrentIcon(string iconPath, int index)
        {
            // extract the icon
            var largeIcons = new IntPtr[1];
            var smallIcons = new IntPtr[1];
            ExtractIconEx(iconPath, index, largeIcons, smallIcons, 1);

            CurrentIcon.Index = index;
            CurrentIcon.Path = iconPath;

            // clean up
            DestroyIcon(largeIcons[0]);
            DestroyIcon(smallIcons[0]);

            return CurrentIcon;
        }

        public IconData? ShowIconPicker()
        {
            return ShowIconPicker(DefaultIconfile);
        }

        public IconData? ShowIconPicker(string iconString)
        {
            IconData icon = new IconData(iconString);
            return ShowIconPicker(icon);
        }

        public IconData? ShowIconPicker(IconData icon)
        {
            return ShowIconPicker(icon.Path, icon.Index);
        }

        public IconData? ShowIconPicker(string iconfile, int index)
        {
            // show the Pick Icon Dialog
            int retval;
            var handle = new WindowInteropHelper(_window).Handle;
            var iconPath = new System.Text.StringBuilder(iconfile, MAX_PATH);
            retval = PickIconDlg(handle, iconPath, iconPath.MaxCapacity, ref index);

            if (retval != 0)
            {
                SetCurrentIcon(iconPath.ToString(), index);
                return CurrentIcon;

            }

            return null;
        }

    }
}
