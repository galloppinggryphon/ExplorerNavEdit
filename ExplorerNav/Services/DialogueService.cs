using System.Windows;

namespace ExplorerNav.Services
{
    interface IDialogueService
    {
        void ShowInfo(string title, string message);
        void ShowWarning(string title, string message);
        void ShowError(string title, string message);
        bool AskYesNo(string title, string message, bool shouldWarn = false);
        public string? ShowDirectoryBrowse(string directoryPath);
    }

    internal class DialogueService : IDialogueService
    {
        public void ShowInfo(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
        }
        public void ShowWarning(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }
        public void ShowError(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        public bool AskYesNo(string title, string message, bool shouldWarn = false)
        {
            var icon = shouldWarn ? MessageBoxImage.Warning : MessageBoxImage.Question;
            var result = MessageBox.Show(message, title, MessageBoxButton.YesNo, icon);
            return result == MessageBoxResult.Yes;
        }

        public string? ShowDirectoryBrowse(string directoryPath)
        {
            //Note: requires <UseWindowsForms>true</UseWindowsForms> in .csproj
            using (System.Windows.Forms.FolderBrowserDialog folderDialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                folderDialog.SelectedPath = directoryPath;

                System.Windows.Forms.DialogResult result = folderDialog.ShowDialog();
                if (result.ToString() == "OK")
                    return folderDialog.SelectedPath;

                return null;
            }
        }
    }
}
