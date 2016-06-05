using Arma3BE.Client.Infrastructure.Events;
using Arma3BE.Client.Infrastructure.Models;
using Arma3BEClient.Libs.ModelCompact;
using Arma3BEClient.Libs.Repositories;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Arma3BE.Client.Modules.MainModule.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IEventAggregator _eventAggregator;

        public MainViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            InitServers();

            OptionsCommand = new DelegateCommand(() =>
            {
                _eventAggregator.GetEvent<ShowOptionsEvent>().Publish(null);
            });

            _eventAggregator.GetEvent<BEServersChangedEvent>().Subscribe((state) =>
            {
                Reload();
            });
        }

        public ICommand OptionsCommand { get; set; }

        public List<ServerInfo> Servers
        {
            get
            {
                using (var repo = new ServerInfoRepository())
                    return repo.GetNotActiveServerInfo().OrderBy(x => x.Name).ToList();
            }
        }

        private void InitServers()
        {
            Reload();
        }

        public void Reload()
        {
            OnPropertyChanged(nameof(Servers));
        }

        public void SetActive(Guid serverId, bool active = false)
        {
            using (var repo = new ServerInfoRepository())
            {
                repo.SetServerInfoActive(serverId, active);
            }
        }
    }
}