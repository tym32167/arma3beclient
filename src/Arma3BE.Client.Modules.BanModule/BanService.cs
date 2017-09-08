using Arma3BE.Client.Infrastructure.Events;
using Arma3BE.Client.Modules.BanModule.Boxes;
using Microsoft.Practices.Unity;
using Prism.Events;
using System;

namespace Arma3BE.Client.Modules.BanModule
{
    public class BanService
    {
        private readonly IUnityContainer _container;

        public BanService(IUnityContainer container, IEventAggregator eventAggregator)
        {
            _container = container;

            eventAggregator.GetEvent<BanUserEvent>()
                .Subscribe(e => ShowBanDialog(e.ServerId, e.PlayerGuid, e.IsOnline, e.PlayerName, e.PlayerNum));

            eventAggregator.GetEvent<KickUserEvent>()
                .Subscribe(e => ShowKickDialog(e.ServerId, e.PlayerNum, e.PlayerGuid, e.PlayerName));

        }

        private void ShowBanDialog(Guid? serverId, string playerGuid, bool isOnline, string playerName,
            string playerNum)
        {

            var w = _container.Resolve<BanPlayerWindow>(
                   new ParameterOverride("serverId", serverId ?? Guid.Empty),
                   new ParameterOverride("playerGuid", playerGuid),
                   new ParameterOverride("isOnline", isOnline),
                   new ParameterOverride("playerName", playerName ?? string.Empty),
                   new ParameterOverride("playerNum", playerNum ?? string.Empty)
                   );
            w.ShowDialog();

            //if (playerNum != null)
            //{
            //    var w = _container.Resolve<BanPlayerWindow>(
            //        new ParameterOverride("serverId", serverId),
            //        new ParameterOverride("playerGuid", playerGuid),
            //        new ParameterOverride("isOnline", isOnline),
            //        new ParameterOverride("playerName", playerName),
            //        new ParameterOverride("playerNum", playerNum)
            //        );
            //    w.ShowDialog();
            //}
            //else
            //{
            //    var w = _container.Resolve<BanPlayerWindow>(
            //       new ParameterOverride("serverId", serverId),
            //       new ParameterOverride("playerGuid", playerGuid),
            //       new ParameterOverride("isOnline", isOnline),
            //       new ParameterOverride("playerName", playerName),
            //        new ParameterOverride("playerNum", string.Empty)
            //       );
            //    w.ShowDialog();
            //}
        }

        private void ShowKickDialog(Guid serverId, int playerNum, string playerGuid, string playerName)
        {
            var w = _container.Resolve<KickPlayerWindow>(
                new ParameterOverride("serverId", serverId),
                new ParameterOverride("playerGuid", playerGuid),
                new ParameterOverride("playerName", playerName),
                new ParameterOverride("playerNum", playerNum));
            w.ShowDialog();
        }
    }
}
