using Arma3BE.Client.Infrastructure.Helpers;
using Arma3BE.Client.Modules.CoreModule.Helpers;
using Arma3BEClient.Common.Core;
using Arma3BEClient.Common.Logging;
using Arma3BEClient.Libs.Core;
using Arma3BEClient.Libs.Core.Settings;
using Arma3BEClient.Libs.EF.Repositories;
using Arma3BEClient.Libs.EF.Settings;
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
            _container.RegisterType<ICacheFactory, CacheFactory>();




            _container.RegisterType<ISettingsStoreSource, SettingsStoreSource>();
            //_container.RegisterType<IPlayerRepository>(
            //    new InjectionFactory(
            //        c =>
            //        //new PlayerRepositoryCache(c.Resolve<PlayerRepository>())
            //        new PlayerRepositoryCacheV2(c.Resolve<PlayerRepository>(), c.Resolve<ICacheFactory>())
            //            ));
            //_container.RegisterType<IServerInfoRepository>(
            //   new InjectionFactory(
            //       c =>
            //           new ServerInfoRepositoryCache(
            //               c.Resolve<ServerInfoRepository>())));

            _container.RegisterType<IAdminRepository, AdminRepository>();
            _container.RegisterType<IBanRepository, BanRepository>();
            _container.RegisterType<IChatRepository, ChatRepository>();
            _container.RegisterType<IReasonRepository, ReasonRepository>();





            _container.RegisterType<IServerInfoRepository, Arma3BEClient.Libs.RavenDB.Repositories.ServerInfoRepository>();
            _container.RegisterType<IPlayerRepository, Arma3BEClient.Libs.RavenDB.Repositories.PlayerRepository>();


            _container.RegisterType<IBanHelper, BanHelper>();
        }
    }
}