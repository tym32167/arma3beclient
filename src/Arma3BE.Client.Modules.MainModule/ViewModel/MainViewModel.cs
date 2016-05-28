using Arma3BE.Client.Infrastructure.Models;
using Arma3BEClient.Libs.ModelCompact;
using Arma3BEClient.Libs.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Arma3BE.Client.Modules.MainModule.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            InitServers();
        }

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