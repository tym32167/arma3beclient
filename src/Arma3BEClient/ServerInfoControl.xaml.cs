using System;
using System.Configuration;
using System.Data.SqlServerCe;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using Arma3BEClient.Common.Logging;
using Arma3BEClient.Lib.ModelCompact;
using Arma3BEClient.Updater.Models;
using Arma3BEClient.ViewModel;



namespace Arma3BEClient
{
    /// <summary>
    /// Interaction logic for ServerInfoControl.xaml
    /// </summary>
    public partial class ServerInfoControl : UserControl
    {
        private ServerMonitorModel _model;


        public ServerInfoControl(ServerInfo serverInfo)
        {
            InitializeComponent();


            _model = new ServerMonitorModel(serverInfo, new Log());
            this.DataContext = _model;
            //_model.Connect();
            Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
        }

        void Dispatcher_ShutdownStarted(object sender, System.EventArgs e)
        {
            Cleanup();
        }

        public void Cleanup()
        {
            if (_model != null)
            {
                _model.Cleanup();
                _model = null;
            }
        }
    }
}
