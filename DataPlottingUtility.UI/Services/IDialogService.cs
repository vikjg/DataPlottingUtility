using System.Windows;

namespace DataPlottingUtility.UI.Services
{
    public interface IDialogService
    {
        string? OpenFileDialog(string filter, string title);
        bool? ShowMessageBox(string message, string title, MessageBoxButton buttons);
        void ShowError(string message, string title = "Error");
        void ShowInfo(string message, string title = "Information");
    }
}

