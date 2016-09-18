using System.Collections.ObjectModel;
using Arma3BE.Client.Infrastructure.Events;
using Arma3BE.Client.Infrastructure.Events.Models;
using Arma3BE.Client.Infrastructure.Models;
using Prism.Events;

namespace Arma3BE.Client.Modules.DevTools.Models
{
    public class LogViewerViewModel : ViewModelBase
    {
        public LogViewerViewModel(IEventAggregator aggregator)
        {
            Messages = new ObservableCollection<LoggerMessage>();


            aggregator.GetEvent<LoggerMessageEvent>().Subscribe(ProcessMessage, ThreadOption.UIThread);

            aggregator.GetEvent<ShowUserInfoEvent>().Subscribe(e =>
            {
                ProcessMessage(new LoggerMessage(LoggerMessage.LogLevel.Info, e.UserGuid));
            }, ThreadOption.UIThread);
        }

        public ObservableCollection<LoggerMessage> Messages { get; set; }


        private void ProcessMessage(LoggerMessage message)
        {
            Messages.Add(message);
        }

        public LoggerMessage SelectedMessage { get; set; }
    }
}