using Arma3BE.Client.Modules.SyncModule.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Arma3BE.Client.Modules.SyncModule.Grids
{
    /// <summary>
    /// Interaction logic for SyncModuleView.xaml
    /// </summary>
    public partial class SyncModuleView : UserControl
    {
        public SyncModuleView()
        {
            InitializeComponent();
        }

        private void PasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            (this.DataContext as SyncModuleViewModel).UserPassword = passwordBox.Password;
        }
    }
}
