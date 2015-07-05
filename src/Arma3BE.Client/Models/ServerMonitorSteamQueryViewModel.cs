using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Input;
using Arma3BEClient.Commands;
using Arma3BEClient.Common.Logging;
using Arma3BEClient.Helpers;
using Arma3BEClient.Steam;
using GalaSoft.MvvmLight;
using System;

namespace Arma3BEClient.Models
{
    public class ServerMonitorSteamQueryViewModel : ViewModelBase
    {
        private readonly ILog _log;
        private IEnumerable<Tuple<string, string>> _serverRules;
        private IEnumerable<Tuple<string, string>> _serverInfo;

        public ICommand ExcecuteCommand { get; set; }

        public string Host { get; set; }
        public int Port { get; set; }

        private ServerPlayers _serverPlayers;
        

        public ServerMonitorSteamQueryViewModel(string host, int port, ILog log)
        {
            _log = log;
            Host = host;
            Port = port + 1;

            RaisePropertyChanged("Host");
            RaisePropertyChanged("Port");

            ExcecuteCommand = new ActionCommand(() => Task.Run(() =>
            {
                var iphost = IPInfo.GetIPAddress(Host);
                var server = new Server(new IPEndPoint(IPAddress.Parse(iphost), Port));

                var settings = new GetServerInfoSettings();
                var rules = server.GetServerRulesSync(settings);
                _serverRules =
                    rules.Select(
                        x =>
                            new Tuple<string, string>(x.Key,
                                x.Value)).ToList();

                var serverInfo =
                    server.GetServerInfoSync(settings);

                var props = serverInfo.GetType().GetProperties();

                _serverInfo =
                    props.Select(
                        x =>
                            new Tuple<string, string>(x.Name,
                                x.GetValue(serverInfo)
                                    .ToString())).ToList();

                _serverPlayers =
                    server.GetServerChallengeSync(settings);


                RaisePropertyChanged("ServerRules");
                RaisePropertyChanged("ServerInfo");
                RaisePropertyChanged("ServerPlayers");

            }),
                () =>
                {
                    var iphost = IPInfo.GetIPAddress(Host);

                    if (string.IsNullOrEmpty(iphost))
                    {
                        return false;
                    }
                    return true;
                });
        }


        public IEnumerable<Tuple<string, string>> ServerRules
        {
            get
            {
                return _serverRules;
            }
        }


        public IEnumerable<Tuple<string, string>> ServerInfo
        {
            get
            {
                return _serverInfo;
            }
        }

        public ServerPlayers ServerPlayers
        {
            get
            {
                return _serverPlayers;
            }
        }
    }
}