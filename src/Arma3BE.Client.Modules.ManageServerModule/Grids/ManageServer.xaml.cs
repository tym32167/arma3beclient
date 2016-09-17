using Arma3BE.Client.Modules.ManageServerModule.Models;
using System.Windows.Controls;
using Prism.Regions;

namespace Arma3BE.Client.Modules.ManageServerModule.Grids
{
    /// <summary>
    ///     Interaction logic for ManageServer.xaml
    /// </summary>
    [ViewSortHint("0500")]
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