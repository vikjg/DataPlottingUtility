using System;
using System.Windows;
using Microsoft.Win32;

namespace DataPlottingUtility.UI.Services
{
    public class DialogService : IDialogService
    {
        public string? OpenFileDialog(string filter, string title)
        {
            try
            {
                var dialog = new OpenFileDialog
                {
                    Filter = filter,
                    Title = title
                };

                return dialog.ShowDialog() == true ? dialog.FileName : null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in OpenFileDialog: {ex.Message}");
                return null;
            }
        }

        public bool? ShowMessageBox(string message, string title, MessageBoxButton buttons)
        {
            try
            {
                var result = MessageBox.Show(message, title, buttons);
                // Convert MessageBoxResult to bool?
                switch (result)
                {
                    case MessageBoxResult.OK:
                    case MessageBoxResult.Yes:
                        return true;
                    case MessageBoxResult.No:
                    case MessageBoxResult.Cancel:
                        return false;
                    default:
                        return null;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in ShowMessageBox: {ex.Message}");
                return null;
            }
        }

        public void ShowError(string message, string title = "Error")
        {
            try
            {
                MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in ShowError: {ex.Message}");
            }
        }

        public void ShowInfo(string message, string title = "Information")
        {
            try
            {
                MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in ShowInfo: {ex.Message}");
            }
        }
    }
}


