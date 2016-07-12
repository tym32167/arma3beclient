using Arma3BE.Client.Infrastructure.Models;
using Arma3BE.Client.Modules.PlayersModule.ViewModel;
using Microsoft.Practices.Unity;
using Prism.Modularity;

namespace Arma3BE.Client.Modules.PlayersModule
{
    public class PlayersModuleInit : IModule
    {
        private static IUnityContainer _container;

        public PlayersModuleInit(IUnityContainer container)
        {
            _container = container;
        }

        public void Initialize()
        {
            _container.RegisterInstance(new PlayerService(_container));
            _container.RegisterType<IPlayerListModelView, PlayerListModelView>();
        }

        public static IUnityContainer Current => _container;
    }
}