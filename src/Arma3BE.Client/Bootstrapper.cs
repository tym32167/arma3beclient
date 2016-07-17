using Arma3BE.Client.Modules.AdminsModule;
using Arma3BE.Client.Modules.BanModule;
using Arma3BE.Client.Modules.BEServerModule;
using Arma3BE.Client.Modules.ChatModule;
using Arma3BE.Client.Modules.IndicatorsModule;
using Arma3BE.Client.Modules.MainModule;
using Arma3BE.Client.Modules.ManageServerModule;
using Arma3BE.Client.Modules.NetModule;
using Arma3BE.Client.Modules.OnlinePlayersModule;
using Arma3BE.Client.Modules.OptionsModule;
using Arma3BE.Client.Modules.PlayersModule;
using Arma3BE.Client.Modules.SteamModule;
using Arma3BEClient.Common.Logging;
using log4net.Config;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;
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

            Application.Current.MainWindow = (Window)Shell;
            Application.Current.MainWindow.Show();
        }

        protected override void ConfigureModuleCatalog()
        {
            base.ConfigureModuleCatalog();

            AddModule(typeof(NetModuleInit));
            AddModule(typeof(OptionsModuleInit));
            AddModule(typeof(BEServerModuleInit));
            AddModule(typeof(PlayersModuleInit));
            AddModule(typeof(BanModuleInit));
            AddModule(typeof(AdminsModuleInit));
            AddModule(typeof(ChatModuleInit));
            AddModule(typeof(ManageServerModuleInit));
            AddModule(typeof(OnlinePlayersModuleInit));
            AddModule(typeof(SteamModuleInit));
            AddModule(typeof(IndicatorsModuleInit));
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

        protected override RegionAdapterMappings ConfigureRegionAdapterMappings()
        {
            // Call base method
            var mappings = base.ConfigureRegionAdapterMappings();
            if (mappings == null) return null;

            MainModuleInit.CreateRegionAdapterMappings(mappings);

            // Add custom mappings
            //mappings.RegisterMapping(typeof(DockingManager),
            //    ServiceLocator.Current.GetInstance<AvalonDockRegionAdapter>());

            // Set return value
            return mappings;
        }
    }
}