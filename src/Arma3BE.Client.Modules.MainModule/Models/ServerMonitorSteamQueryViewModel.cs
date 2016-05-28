using Arma3BE.Client.Infrastructure.Commands;
using Arma3BEClient.Common.Logging;
using Arma3BEClient.Steam;
using Arma3BE.Client.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Arma3BE.Client.Modules.MainModule.Models
{
    public class ServerMonitorSteamQueryViewModel : ViewModelBase
    {
        private readonly ILog _log;

        public ServerMonitorSteamQueryViewModel(string host, int port, ILog log)
        {
            _log = log;
            Host = host;
            Port = port + 1;

            OnPropertyChanged("Host");
            OnPropertyChanged("Port");

            ExcecuteCommand = new ActionCommand(() => Task.Run(() =>
            {
                var iphost = host;
                var server = new Arma3BEClient.Steam.Server(new IPEndPoint(IPAddress.Parse(iphost), Port));

                var settings = new GetServerInfoSettings();
                var rules = server.GetServerRulesSync(settings);
                ServerRules =
                    rules.Select(
                        x =>
                            new Tuple<string, string>(x.Key,
                                x.Value)).ToList();

                var serverInfo =
                    server.GetServerInfoSync(settings);

                var props = serverInfo.GetType().GetProperties();

                ServerInfo =
                    props.Select(
                        x =>
                            new Tuple<string, string>(x.Name,
                                x.GetValue(serverInfo)
                                    .ToString())).ToList();

                ServerPlayers =
                    server.GetServerChallengeSync(settings);


                OnPropertyChanged("ServerRules");
                OnPropertyChanged("ServerInfo");
                OnPropertyChanged("ServerPlayers");
            }),
                () =>
                {
                    var iphost = host;

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