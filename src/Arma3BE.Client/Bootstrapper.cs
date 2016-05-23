using Arma3BE.Client.Modules.MainModule;
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

            Type moduleCType = typeof(MainModuleInit);
            ModuleCatalog.AddModule(
              new ModuleInfo()
              {
                  ModuleName = moduleCType.Name,
                  ModuleType = moduleCType.AssemblyQualifiedName,
              });
        }
    }
}