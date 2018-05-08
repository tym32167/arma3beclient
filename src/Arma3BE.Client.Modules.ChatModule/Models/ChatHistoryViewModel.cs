using Arma3BE.Client.Infrastructure.Commands;
using Arma3BE.Client.Infrastructure.Extensions;
using Arma3BE.Client.Infrastructure.Models;
using Arma3BEClient.Libs.Core;
using Arma3BEClient.Libs.EF.Repositories;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Arma3BEClient.Libs.Core.Model;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace Arma3BE.Client.Modules.ChatModule.Models
{
    public class ChatHistoryViewModel : ViewModelBase
    {
        private readonly IServerInfoRepository _repository;

        public ChatHistoryViewModel(Guid serverId, IServerInfoRepository repository)
        {
            _repository = repository;
            FilterCommand = new ActionCommand(async () =>
            {
                try
                {
                    IsBusy = true;
                    await UpdateLogAsync();
                    RaisePropertyChanged(nameof(Log));
                }
                finally
                {
                    IsBusy = false;
                }
            });


            Init(serverId);
        }

        public async Task Init(Guid serverId)
        {
            ServerList = (await _repository.GetServerInfoAsync()).OrderBy(x => x.Name).ToList();
            SelectedServers = serverId.ToString();

            RaisePropertyChanged(nameof(ServerList));
            RaisePropertyChanged(nameof(SelectedServers));
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                RaisePropertyChanged();
            }
        }

        private async Task UpdateLogAsync()
        {
            using (var dc = ServiceLocator.Current.TryResolve<IChatRepository>())
            {
                var log = await dc.GetChatLogsAsync(SelectedServers, StartDate.LocalToUtcFromSettings(), EndDate.LocalToUtcFromSettings(), Filter);

                Log = log.OrderBy(x => x.Date).Select(x => new ChatView
                {
                    Id = x.Id,
                    Date = x.Date,
                    ServerName = x.ServerInfo.Name,
                    Text = x.Text
                }).ToList();
            }
        }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Filter { get; set; }

        public IEnumerable<ChatView> Log { get; private set; }


        public IEnumerable<ServerInfoDto> ServerList { get; set; }

        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
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