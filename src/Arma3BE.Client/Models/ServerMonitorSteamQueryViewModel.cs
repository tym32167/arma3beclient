using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Input;
using Arma3BEClient.Commands;
using Arma3BEClient.Common.Dns;
using Arma3BEClient.Common.Logging;
using Arma3BEClient.Helpers;
using Arma3BEClient.Steam;
using GalaSoft.MvvmLight;

namespace Arma3BEClient.Models
{
    public class ServerMonitorSteamQueryViewModel : ViewModelBase
    {
        private readonly ILog _log;

        public ServerMonitorSteamQueryViewModel(string host, int port, ILog log)
        {
            _log = log;
            Host = host;
            Port = port + 1;

            RaisePropertyChanged("Host");
            RaisePropertyChanged("Port");

            ExcecuteCommand = new ActionCommand(() => Task.Run(() =>
            {
                var iphost = DnsService.GetIpAddress(Host);
                var server = new Server(new IPEndPoint(IPAddress.Parse(iphost), Port));

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


                RaisePropertyChanged("ServerRules");
                RaisePropertyChanged("ServerInfo");
                RaisePropertyChanged("ServerPlayers");
            }),
                () =>
                {
                    var iphost = DnsService.GetIpAddress(Host);

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