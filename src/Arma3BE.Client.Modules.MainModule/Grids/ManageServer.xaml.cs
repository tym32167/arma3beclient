using System.Windows.Controls;
using Arma3BE.Client.Modules.MainModule.Models;

namespace Arma3BE.Client.Modules.MainModule.Grids
{
    /// <summary>
    ///     Interaction logic for ManageServer.xaml
    /// </summary>
    public partial class ManageServer : UserControl
    {
        public ManageServer()
        {
            InitializeComponent();
        }

        private ServerMonitorManageServerViewModel Model
        {
            get { return DataContext as ServerMonitorManageServerViewModel; }
        }
    }
}