using System;
using System.Windows.Controls;
using Arma3BEClient.Common.Logging;
using Arma3BEClient.Libs.ModelCompact;
using Arma3BEClient.ViewModel;

namespace Arma3BEClient
{
    /// <summary>
    ///     Interaction logic for ServerInfoControl.xaml
    /// </summary>
    public partial class ServerInfoControl : UserControl, IDisposable
    {
        private bool _disposed;
        private ServerMonitorModel _model;

        public ServerInfoControl(ServerInfo serverInfo)
        {
            InitializeComponent();

            _model = new ServerMonitorModel(serverInfo, new Log());
            DataContext = _model;
            Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispatcher_ShutdownStarted(object sender, EventArgs e)
        {
            Cleanup();
        }

        public void Cleanup()
        {
            Dispose();
        }

        ~ServerInfoControl()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                DisposeManagedResources();
            }
            DisposeUnManagedResources();
            _disposed = true;
        }

        protected virtual void DisposeManagedResources()
        {
            if (_model != null)
            {
                _model.Dispose();
                _model.Cleanup();
                _model = null;
            }
        }

        protected virtual void DisposeUnManagedResources()
        {
        }
    }
}