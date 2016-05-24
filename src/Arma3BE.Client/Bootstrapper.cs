using Arma3BE.Client.Modules.MainModule;
using Arma3BE.Client.Modules.NetModule;
using Arma3BEClient.Common.Logging;
using log4net.Config;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Unity;
using System;
using System.Windows;

namespace Arma3BEClient
{
    public class Bootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<Shell>();
        }

        protected override void InitializeShell()
        {
            XmlConfigurator.Configure();

            base.InitializeShell();

            App.Current.MainWindow = (Window)Shell;
            App.Current.MainWindow.Show();
        }

        protected override void ConfigureModuleCatalog()
        {
            base.ConfigureModuleCatalog();

            AddModule(typeof(NetModuleInit));
            AddModule(typeof(MainModuleInit));
        }

        private void AddModule(Type initType)
        {
            ModuleCatalog.AddModule(
              new ModuleInfo()
              {
                  ModuleName = initType.Name,
                  ModuleType = initType.AssemblyQualifiedName,
              });
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();
            Container.RegisterType<ILog, Log>();
        }
    }
}