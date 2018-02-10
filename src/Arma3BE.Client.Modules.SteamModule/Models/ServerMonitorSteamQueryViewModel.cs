using Arma3BE.Client.Infrastructure;
using Arma3BE.Client.Infrastructure.Commands;
using Arma3BE.Client.Infrastructure.Models;
using Arma3BEClient.Steam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Input;
using Arma3BEClient.Libs.EF.Repositories;

// ReSharper disable VirtualMemberCallInConstructor
// ReSharper disable ExplicitCallerInfoArgument
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Arma3BE.Client.Modules.SteamModule.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ServerMonitorSteamQueryViewModel : ViewModelBase
    {
        private readonly IIpService _ipService;
        private bool _isBisy;


        public ServerMonitorSteamQueryViewModel(ServerInfoDto serverInfo, IIpService ipService)
        {
            _ipService = ipService;
            Host = serverInfo.Host;
            Port = serverInfo.SteamPort;

            RaisePropertyChanged(nameof(Host));
            RaisePropertyChanged(nameof(Port));

            ExcecuteCommand = new ActionCommand(() => Task.Run(() =>
            {
                try
                {
                    IsBisy = true;
                    GetStat();
                }
                finally
                {
                    IsBisy = false;
                }
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

        private void GetStat()
        {
            var iphost = _ipService.GetIpAddress(Host);
            var server = new Server(new IPEndPoint(IPAddress.Parse(iphost), Port));

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


            RaisePropertyChanged(nameof(ServerRules));
            RaisePropertyChanged(nameof(ServerInfo));
            RaisePropertyChanged(nameof(ServerPlayers));
        }

        public string Title => "Steam Query";

        public bool IsBisy
        {
            get { return _isBisy; }
            set
            {
                _isBisy = value;
                RaisePropertyChanged();
            }
        }

        public ICommand ExcecuteCommand { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public IEnumerable<Tuple<string, string>> ServerRules { get; private set; }
        public IEnumerable<Tuple<string, string>> ServerInfo { get; private set; }
        public ServerPlayers ServerPlayers { get; private set; }
    }
}