using Arma3BE.Client.Infrastructure.Events;
using Arma3BE.Client.Infrastructure.Models;
using Arma3BE.Client.Modules.ChatModule.Chat;
using Prism.Events;
using System.Windows;
using System.Windows.Controls;

namespace Arma3BE.Client.Modules.ChatModule
{
    public class ChatService
    {
        public ChatService(IEventAggregator eventAggregator)
        {
            eventAggregator.GetEvent<CreateViewEvent<IServerMonitorChatViewModel>>().Subscribe(e =>
            {
                CreateView(e.Parent, e.ViewModel);
            });
        }

        private void CreateView(ContentControl parent, IServerMonitorChatViewModel model)
        {
            var dispatcher = Application.Current.Dispatcher;

            if (dispatcher.CheckAccess())
            {
                FrameworkElement control = new ChatControl();
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