using System.Windows;
using System.Windows.Controls;
using Arma3BE.Client.Infrastructure.Events;
using Arma3BE.Client.Infrastructure.Models;
using Arma3BE.Client.Modules.SteamModule.Grids;
using Prism.Events;

namespace Arma3BE.Client.Modules.SteamModule
{
    public class SteamService
    {
        public SteamService(IEventAggregator eventAggregator)
        {
            eventAggregator.GetEvent<CreateViewEvent<IServerMonitorSteamQueryViewModel>>().Subscribe(e =>
            {
                CreateView(e.Parent, e.ViewModel);
            });
        }

        private void CreateView(ContentControl parent, IServerMonitorSteamQueryViewModel model)
        {
            var dispatcher = Application.Current.Dispatcher;

            if (dispatcher.CheckAccess())
            {
                FrameworkElement control = new SteamQuery();
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