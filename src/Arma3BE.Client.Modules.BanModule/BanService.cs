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
                .Subscribe(e => ShowBanDialog(e.BEServer, e.PlayerGuid, e.IsOnline, e.PlayerName, e.PlayerNum));

            _eventAggregator.GetEvent<KickUserEvent>()
                .Subscribe(e => ShowKickDialog(e.BEServer, e.PlayerNum, e.PlayerGuid, e.PlayerName));

            _eventAggregator.GetEvent<CreateViewEvent<IServerMonitorBansViewModel>>().Subscribe(e =>
            {
                CreateBanView(e.Parent, e.ViewModel);
            });
        }

        public void ShowBanDialog(IBEServer beServer, string playerGuid, bool isOnline, string playerName,
            string playerNum)
        {
            if (playerNum != null)
            {
                var w = _container.Resolve<BanPlayerWindow>(
                    new ParameterOverride("beServer", beServer),
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
                   new ParameterOverride("beServer", beServer),
                   new ParameterOverride("playerGuid", playerGuid),
                   new ParameterOverride("isOnline", isOnline),
                   new ParameterOverride("playerName", playerName),
                    new ParameterOverride("playerNum", string.Empty)
                   );
                w.ShowDialog();
            }
        }

        public void ShowKickDialog(IBEServer beServer, int playerNum, string playerGuid, string playerName)
        {
            var w = _container.Resolve<KickPlayerWindow>(
                new ParameterOverride("beServer", beServer),
                new ParameterOverride("playerGuid", playerGuid),
                new ParameterOverride("playerName", playerName),
                new ParameterOverride("playerNum", playerNum));
            w.ShowDialog();
        }

        public void CreateBanView(ContentControl parent, IServerMonitorBansViewModel model)
        {
            var dispatcher = Application.Current.Dispatcher;

            FrameworkElement control = null;

            if (dispatcher.CheckAccess())
            {
                control = new BansControl();
                control.DataContext = model;
                parent.Content = control;
            }
            else
            {
                dispatcher.Invoke(() =>
                {
                    CreateBanView(parent, model);
                });
            }
        }
    }
}
