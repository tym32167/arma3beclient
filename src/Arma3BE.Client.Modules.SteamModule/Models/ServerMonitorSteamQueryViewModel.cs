using Arma3BE.Client.Infrastructure.Commands;
using Arma3BE.Client.Infrastructure.Models;
using Arma3BEClient.Common.Logging;
using Arma3BEClient.Steam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Input;
using Arma3BE.Client.Infrastructure;
using Arma3BEClient.Libs.ModelCompact;

namespace Arma3BE.Client.Modules.SteamModule.Models
{
    public class ServerMonitorSteamQueryViewModel : ViewModelBase, IServerMonitorSteamQueryViewModel
    {
        private readonly ILog _log;
        private readonly IIpService _ipService;

        public ServerMonitorSteamQueryViewModel(ServerInfo serverInfo, ILog log, IIpService ipService)
        {
            _log = log;
            _ipService = ipService;
            Host = serverInfo.Host;
            Port = serverInfo.Port + 1;

            OnPropertyChanged("Host");
            OnPropertyChanged("Port");

            ExcecuteCommand = new ActionCommand(() => Task.Run(() =>
            {
                var iphost = ipService.GetIpAddress(Host);
                var server = new Arma3BEClient.Steam.Server(new IPEndPoint(IPAddress.Parse(iphost), Port));

                var settings = new GetServerInfoSettings();
                var rules = server.GetServerRulesSync(settings);
                ServerRules =
                    rules.Select(
                        x =>
                            new Tuple<string, string>(x.Key,
                                x.Value)).ToList();

                var serverInfoR =
                    server.GetServerInfoSync(settings);

                var props = serverInfoR.GetType().GetProperties();

                ServerInfo =
                    props.Select(
                        x =>
                            new Tuple<string, string>(x.Name,
                                x.GetValue(serverInfoR)
                                    .ToString())).ToList();

                ServerPlayers =
                    server.GetServerChallengeSync(settings);


                OnPropertyChanged("ServerRules");
                OnPropertyChanged("ServerInfo");
                OnPropertyChanged("ServerPlayers");
            }),
                () =>
                {
                    var iphost = Host;

                    if (string.IsNullOrEmpty(iphost))
                    {
                        return false;
                    }
                    return true;
                });
        }

        public ICommand ExcecuteCommand { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public IEnumerable<Tuple<string, string>> ServerRules { get; private set; }
        public IEnumerable<Tuple<string, string>> ServerInfo { get; private set; }
        public ServerPlayers ServerPlayers { get; private set; }
    }
}