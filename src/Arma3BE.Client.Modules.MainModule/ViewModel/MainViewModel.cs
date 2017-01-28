using Arma3BE.Client.Infrastructure.Events;
using Arma3BE.Client.Infrastructure.Models;
using Arma3BEClient.Libs.ModelCompact;
using Arma3BEClient.Libs.Repositories;
using Prism.Commands;
using Prism.Events;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ExplicitCallerInfoArgument

namespace Arma3BE.Client.Modules.MainModule.ViewModel
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel(IEventAggregator eventAggregator)
        {
            var eventAggregator1 = eventAggregator;
            InitServers();

            OptionsCommand = new DelegateCommand(() =>
            {
                eventAggregator1.GetEvent<ShowOptionsEvent>().Publish(null);
            });

            eventAggregator1.GetEvent<BEServersChangedEvent>().Subscribe((state) =>
            {
                Reload();
            });
        }

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public ICommand OptionsCommand { get; set; }

        // ReSharper disable once MemberCanBeMadeStatic.Global
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
    }
}