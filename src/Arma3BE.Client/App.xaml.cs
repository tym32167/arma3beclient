using System.Windows;
using System.Windows.Threading;
using Arma3BEClient.Common.Logging;
using log4net.Config;

namespace Arma3BEClient
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly ILog _logger = new Log();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            XmlConfigurator.Configure();
            _logger.Info("Startup");
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            _logger.Info("Exit");
        }

        private void App_OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            _logger.Fatal(e.Exception);
            e.Handled = true;
        }
    }
}