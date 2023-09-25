using System.Windows;
using WinForm = System.Windows.Forms;
using WpfForm = Microsoft.Win32;


namespace ExplorerNav.Services
{
    interface IDialogueService
    {
        void ShowInfo(string title, string message);
        void ShowWarning(string title, string message);
        void ShowError(string title, string message);
        bool AskYesNo(string title, string message, bool shouldWarn = false);
        public string? ShowDirectoryBrowse(string directoryPath);
        public string? ShowSaveFile(string? fileName = "", string? path = "", string? filter = "");
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

        public string? ShowSaveFile(string? fileName = "", string? path = "", string? filter = "")
        {
            var dialogue = NewSaveFileDialogue(filter);

            if (fileName != null) dialogue.FileName = fileName;
            if (path != null) dialogue.InitialDirectory = path;

            if (dialogue.ShowDialog() == true)
            {
                return dialogue.FileName;
            }

            return null;
        }

        public string? ShowOpenFile(string? fileName = "", string? path = "", string? filter = "")
        {
            var dialogue = new WpfForm.OpenFileDialog(); //NewSaveFileDialogue(filter);

            //WpfForm.OpenFileDialog dialogue = new();
            dialogue.Filter = filter ?? ""; // "Text file (*.txt)|*.txt|C# file (*.cs)|*.cs";

            if (fileName != null) dialogue.FileName = fileName;
            if (path != null) dialogue.InitialDirectory = path;

            if (dialogue.ShowDialog() == true)
            {
                return dialogue.FileName;
            }

            return null;
        }

        public string? ShowDirectoryBrowse(string directoryPath)
        {
            //Note: requires <UseWindowsForms>true</UseWindowsForms> in .csproj
            using (WinForm.FolderBrowserDialog folderDialog = new())
            {
                folderDialog.SelectedPath = directoryPath;

                WinForm.DialogResult result = folderDialog.ShowDialog();
                if (result.ToString() == "OK")
                    return folderDialog.SelectedPath;

                return null;
            }
        }

        private WpfForm.SaveFileDialog NewSaveFileDialogue(string filter)
        {
            WpfForm.SaveFileDialog dialogue = new();
            dialogue.Filter = filter ?? "";
            return dialogue;
        }

        private WpfForm.OpenFileDialog NewOpenFileDialogue(string filter)
        {
            WpfForm.OpenFileDialog dialogue = new();
            dialogue.Filter = filter ?? "";
            return dialogue;
        }
    }
}
