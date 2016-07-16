using System;
using Arma3BE.Client.Infrastructure.Events;
using Arma3BE.Client.Infrastructure.Models;
using Arma3BE.Client.Modules.BanModule.Boxes;
using Arma3BE.Client.Modules.BanModule.Grids;
using Arma3BE.Server.Abstract;
using Microsoft.Practices.Unity;
using Prism.Events;
using System.Windows;
using System.Windows.Controls;

namespace Arma3BE.Client.Modules.BanModule
{
    public class BanService
    {
        private readonly IUnityContainer _container;
        private readonly IEventAggregator _eventAggregator;

        public BanService(IUnityContainer container, IEventAggregator eventAggregator)
        {
            _container = container;
            _eventAggregator = eventAggregator;

            _eventAggregator.GetEvent<BanUserEvent>()
                .Subscribe(e => ShowBanDialog(e.ServerId, e.PlayerGuid, e.IsOnline, e.PlayerName, e.PlayerNum));

            _eventAggregator.GetEvent<KickUserEvent>()
                .Subscribe(e => ShowKickDialog(e.ServerId, e.PlayerNum, e.PlayerGuid, e.PlayerName));
           
        }

        public void ShowBanDialog(Guid serverId, string playerGuid, bool isOnline, string playerName,
            string playerNum)
        {
            if (playerNum != null)
            {
                var w = _container.Resolve<BanPlayerWindow>(
                    new ParameterOverride("serverId", serverId),
                    new ParameterOverride("playerGuid", playerGuid),
                    new ParameterOverride("isOnline", isOnline),
                    new ParameterOverride("playerName", playerName),
                    new ParameterOverride("playerNum", playerNum)
                    );
                w.ShowDialog();
            }
            else
            {
                var w = _container.Resolve<BanPlayerWindow>(
                   new ParameterOverride("serverId", serverId),
                   new ParameterOverride("playerGuid", playerGuid),
                   new ParameterOverride("isOnline", isOnline),
                   new ParameterOverride("playerName", playerName),
                    new ParameterOverride("playerNum", string.Empty)
                   );
                w.ShowDialog();
            }
        }

        public void ShowKickDialog(Guid serverId, int playerNum, string playerGuid, string playerName)
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
