using Arma3BE.Client.Infrastructure.Events;
using Arma3BE.Client.Infrastructure.Models;
using Arma3BE.Client.Modules.ManageServerModule.Grids;
using Prism.Events;
using System.Windows;
using System.Windows.Controls;

namespace Arma3BE.Client.Modules.ManageServerModule
{
    public class ManageServerService
    {
        public ManageServerService(IEventAggregator eventAggregator)
        {
            eventAggregator.GetEvent<CreateViewEvent<IServerMonitorManageServerViewModel>>().Subscribe(e =>
            {
                CreateView(e.Parent, e.ViewModel);
            });
        }

        private void CreateView(ContentControl parent, IServerMonitorManageServerViewModel model)
        {
            var dispatcher = Application.Current.Dispatcher;

            if (dispatcher.CheckAccess())
            {
                FrameworkElement control = new ManageServer();
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