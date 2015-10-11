using System.Windows;
using System.Windows.Threading;
using Arma3BEClient.Common.Logging;
using Castle.Windsor;
using Castle.Windsor.Installer;
using log4net.Config;

namespace Arma3BEClient
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly ILog _logger = new Log();
        private WindsorContainer _container;

        public WindsorContainer Container
        {
            get { return _container; }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            XmlConfigurator.Configure();
            _logger.Info("Startup");
            ConfigureContainer();
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

        private void ConfigureContainer()
        {
            _container = new WindsorContainer();
            _container.Install(FromAssembly.This());
        }
    }
}