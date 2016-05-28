using Arma3BE.Client.Infrastructure.Commands;
using Arma3BE.Client.Infrastructure.Models;
using Arma3BEClient.Libs.ModelCompact;
using Arma3BEClient.Libs.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Arma3BE.Client.Modules.MainModule.Models
{
    public class ChatHistoryViewModel : ViewModelBase
    {
        private readonly Guid _serverId;

        private readonly Lazy<IEnumerable<ServerInfo>> _serverList = new Lazy<IEnumerable<ServerInfo>>(() =>
        {
            using (var repo = new ServerInfoRepository())
            {
                return repo.GetServerInfo().OrderBy(x => x.Name).ToList();
            }
        });

        public ChatHistoryViewModel(Guid serverId)
        {
            _serverId = serverId;

            FilterCommand = new ActionCommand(() => OnPropertyChanged("Log"));

            SelectedServers = serverId.ToString();
        }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Filter { get; set; }

        public IEnumerable<ChatView> Log
        {
            get
            {
                using (var dc = new ChatRepository())
                {
                    var log = dc.GetChatLogs(SelectedServers, StartDate, EndDate, Filter);

                    return log.OrderBy(x => x.Date).Select(x => new ChatView
                    {
                        Id = x.Id,
                        Date = x.Date,
                        ServerName = x.ServerInfo.Name,
                        Text = x.Text
                    }).ToList();
                }
            }
        }


        public IEnumerable<ServerInfo> ServerList
        {
            get { return _serverList.Value; }
        }

        public string SelectedServers { get; set; }
        public ICommand FilterCommand { get; set; }
    }

    public class ChatView
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string ServerName { get; set; }
        public DateTime Date { get; set; }
    }
}