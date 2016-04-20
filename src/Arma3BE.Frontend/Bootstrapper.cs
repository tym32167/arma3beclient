using Arma3BE.Frontend.Modules.Views;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Unity;
using System;
using System.Windows;

namespace Arma3BE.Frontend
{
    internal class Bootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<Shell>();
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();

            App.Current.MainWindow = (Window)Shell;
            App.Current.MainWindow.Show();
        }

        protected override void ConfigureModuleCatalog()
        {
            base.ConfigureModuleCatalog();

            Type moduleCType = typeof(ViewsModuleInit);
            ModuleCatalog.AddModule(
              new ModuleInfo()
              {
                  ModuleName = moduleCType.Name,
                  ModuleType = moduleCType.AssemblyQualifiedName,
              });
        }
    }
}