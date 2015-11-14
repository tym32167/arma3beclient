using System.Windows.Controls;
using Arma3BEClient.Models;

namespace Arma3BEClient.Grids
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