using Arma3BE.Client.Infrastructure;
using Arma3BE.Client.Infrastructure.Models;
using Arma3BE.Client.Modules.BanModule.Boxes;
using Arma3BE.Client.Modules.BanModule.Grids;
using Arma3BE.Server.Abstract;
using Microsoft.Practices.Unity;
using System.Windows;

namespace Arma3BE.Client.Modules.BanModule
{
    public class BanService : IBanService
    {
        private readonly IUnityContainer _container;

        public BanService(IUnityContainer container)
        {
            _container = container;
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

        public object CreateBanView(IServerMonitorBansViewModel model)
        {
            var dispatcher = Application.Current.Dispatcher;

            FrameworkElement control = null;

            if (dispatcher.CheckAccess())
            {
                control = new BansControl();
                control.DataContext = model;
            }
            else
            {
                dispatcher.Invoke(() =>
                {
                    control = new BansControl();
                    control.DataContext = model;
                });
            }
            
            return control;
        }
    }
}
