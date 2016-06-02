using Arma3BE.Client.Modules.PlayersModule.Boxes;
using Arma3BE.Client.Modules.PlayersModule.Models;
using Microsoft.Practices.Unity;

namespace Arma3BE.Client.Modules.PlayersModule.Dialogs
{
    public interface IPlayerViewService
    {
        void ShowDialog(string userGuid);
    }

    public class PlayerViewService : IPlayerViewService
    {
        private readonly IUnityContainer _container;

        public PlayerViewService(IUnityContainer container)
        {
            _container = container;
        }

        public void ShowDialog(string userGuid)
        {
            var model = _container.Resolve<PlayerViewModel>(new ParameterOverride("userGuid", userGuid));

            if (model.Player != null)
            {
                var window = _container.Resolve<PlayerViewWindow>(new ParameterOverride("model", model));
                window.ShowDialog();
            }
        }
    }
}