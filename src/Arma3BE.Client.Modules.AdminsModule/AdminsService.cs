using Arma3BE.Client.Infrastructure.Events;
using Arma3BE.Client.Infrastructure.Models;
using Arma3BE.Client.Modules.AdminsModule.Grids;
using Prism.Events;
using System.Windows;
using System.Windows.Controls;

namespace Arma3BE.Client.Modules.AdminsModule
{
    public class AdminsService
    {
        public AdminsService(IEventAggregator eventAggregator)
        {
            eventAggregator.GetEvent<CreateViewEvent<IServerMonitorAdminsViewModel>>().Subscribe(e =>
            {
                CreateView(e.Parent, e.ViewModel);
            });
        }

        private void CreateView(ContentControl parent, IServerMonitorAdminsViewModel model)
        {
            var dispatcher = Application.Current.Dispatcher;

            if (dispatcher.CheckAccess())
            {
                FrameworkElement control = new AdminsControl();
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