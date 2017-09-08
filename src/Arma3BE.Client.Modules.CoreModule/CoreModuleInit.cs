using Arma3BE.Client.Infrastructure.Helpers;
using Arma3BE.Client.Modules.CoreModule.Helpers;
using Arma3BEClient.Common.Logging;
using Arma3BEClient.Libs.Repositories;
using Arma3BEClient.Libs.Tools;
using Microsoft.Practices.Unity;
using Prism.Modularity;

namespace Arma3BE.Client.Modules.CoreModule
{
    public class CoreModuleInit : IModule
    {
        private readonly IUnityContainer _container;

        public CoreModuleInit(IUnityContainer container)
        {
            _container = container;
        }

        public void Initialize()
        {
            _container.RegisterType<ILog, Log>();


            _container.RegisterType<ISettingsStoreSource, SettingsStoreSource>();


            _container.RegisterType<IPlayerRepository>(
                new InjectionFactory(
                    c =>
                        new PlayerRepositoryCache(
                            c.Resolve<PlayerRepository>())));


            _container.RegisterType<IServerInfoRepository>(
               new InjectionFactory(
                   c =>
                       new ServerInfoRepositoryCache(
                           c.Resolve<ServerInfoRepository>())));


            _container.RegisterType<IBanHelper, BanHelper>();
        }
    }
}