using Arma3BE.Client.Infrastructure.Events;
using Arma3BE.Client.Infrastructure.Models;
using Arma3BE.Client.Modules.OnlinePlayersModule.Grids;
using Prism.Events;
using System.Windows;
using System.Windows.Controls;

namespace Arma3BE.Client.Modules.OnlinePlayersModule
{
    public class OnlinePlayerService
    {
        public OnlinePlayerService(IEventAggregator eventAggregator)
        {
            eventAggregator.GetEvent<CreateViewEvent<IServerMonitorPlayerViewModel>>().Subscribe(e =>
            {
                CreateView(e.Parent, e.ViewModel);
            });
        }

        private void CreateView(ContentControl parent, IServerMonitorPlayerViewModel model)
        {
            var dispatcher = Application.Current.Dispatcher;

            if (dispatcher.CheckAccess())
            {
                FrameworkElement control = new OnlinePlayers();
                control.DataContext = model;
                parent.Content = control;
            }
            else
            {
                dispatcher.Invoke(() =>
                {
                    CreateView(parent, model);
                });
            }
        }
    }
}