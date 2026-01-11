using System.Windows;
using System.Windows.Controls;
using DataPlottingUtility.UI.ViewModels;

namespace DataPlottingUtility.UI.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow(MainWindowViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void XAxisChannelChanged(object sender, SelectionChangedEventArgs e)
        {
            // Force update when selection changes
            var viewModel = (MainWindowViewModel)DataContext;
            viewModel.OnXAxisChannelChanged();
        }
    }
}

