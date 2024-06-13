using ExplorerNav.Services;
using ExplorerNav.ViewModels;
using System.Windows;

namespace ExplorerNav
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IconPicker IconDialogue { get; init; }
        private MainVM Main { get; } = new();

        public MainWindow()
        {
            DataContext = this.Main;

            InitializeComponent();

            IconDialogue = new(this);

            //var targetWindow = Application.Current.Windows
            //    .Cast<Window>()
            //    .Where(window => window.Name == String.Concat(foo.Name, "Window"))
            //    .DefaultIfEmpty(null)
            //    .Single();


            //NavEdit = new NavEditor();
            //NavItems = new()
            //{
            //    new NavItem("Abc", "path", "x123")
            //};
        }

        private void ShowIconPicker(string iconString)
        {
            var icon = IconDialogue.ShowIconPicker(iconString);

            if (icon != null)
            {
                Main.SetCurrentItemIcon(icon);
            }
        }

        private void BtnSelectIcon_Click(object sender, RoutedEventArgs e)
        {
            ShowIconPicker(Main.CurrentItem.Icon);
        }

        private void BtnResetIconPicker_Click(object sender, RoutedEventArgs e)
        {
            Main.SetCurrentItemIcon();
        }

        private void BtnLoadFromRegistry_Click(object sender, RoutedEventArgs e)
        {
            Main.ReadNavItemsFromRegistry();
        }

        private void BtnNewItem_Click(object sender, RoutedEventArgs e)
        {
            Main.NewItem();
        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            Main.RemoveCurrentItem();
        }

        private void BtnUnregister_Click(object sender, RoutedEventArgs e)
        {
            Main.UnregisterNavItem();
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            Main.RegisterNavItem();
        }

        private void BtnAbout_Click(object sender, RoutedEventArgs e)
        {
            Main.ShowAboutWindow();
        }

        private void BtnBrowse_Click(object sender, RoutedEventArgs e)
        {
            Main.ShowBrowseDirectory();
        }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            Main.Export();
        }

        private void BtnImport_Click(object sender, RoutedEventArgs e)
        {
            Main.Import();
        }
    }
}
