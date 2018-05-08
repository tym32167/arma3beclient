using Arma3BE.Client.Infrastructure;
using Arma3BE.Client.Infrastructure.Commands;
using Arma3BE.Client.Infrastructure.Events.BE;
using Arma3BE.Client.Infrastructure.Models;
using Arma3BE.Server;
using Arma3BE.Server.Models;
using Arma3BEClient.Libs.Repositories;
using Arma3BEClient.Steam;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Arma3BE.Client.Modules.ManageServerModule.Models
{
    public class ServerMonitorManageServerViewModel : ViewModelBase
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IIpService _ipService;
        private readonly Guid _serverId;
        private IEnumerable<Mission> _missions;
        private Mission _selectedMission;
        private bool _showWarning;

        public ServerMonitorManageServerViewModel(ServerInfoDto serverInfo, IEventAggregator eventAggregator,
            IIpService ipService)
        {
            _serverId = serverInfo.Id;
            _eventAggregator = eventAggregator;
            _ipService = ipService;

            _eventAggregator.GetEvent<BEMessageEvent<BEItemsMessage<Mission>>>().Subscribe(e =>
            {
                if (e.ServerId == _serverId)
                    Missions = e.Items;
            });

            SetMissionCommand = new ActionCommand(() =>
                {
                    var m = SelectedMission;
                    if (m != null && ConfirmNotSupported(CommandType.Mission))
                    {
                        var mn = m.Name;
                        SendCommand(CommandType.Mission, mn);
                    }
                },
                () => SelectedMission != null);

            RefreshCommand = new ActionCommand(() => SendCommand(CommandType.Missions));

            InitCommand = new ActionCommand(() => { SendCommandWithConfirmation(CommandType.Init); });
            ShutdownCommand = new ActionCommand(() => { SendCommandWithConfirmation(CommandType.Shutdown); });
            ReassignCommand = new ActionCommand(() => { SendCommandWithConfirmation(CommandType.Reassign); });
            RestartCommand = new ActionCommand(() => { SendCommandWithConfirmation(CommandType.Restart); });

            RestartServerCommand = new ActionCommand(() => { SendCommandWithConfirmation(CommandType.RestartServer); });

            LockCommand = new ActionCommand(() => { SendCommandWithConfirmation(CommandType.Lock); });
            UnlockCommand = new ActionCommand(() => { SendCommandWithConfirmation(CommandType.Unlock); });

            LoadBansCommand = new ActionCommand(() => { SendCommandWithConfirmation(CommandType.LoadBans); });
            LoadScriptsCommand = new ActionCommand(() => { SendCommandWithConfirmation(CommandType.LoadScripts); });
            LoadEventsCommand = new ActionCommand(() => { SendCommandWithConfirmation(CommandType.LoadEvents); });

            ShowWarning = false;
            Task.Run(() => CheckServer(serverInfo));
        }

        private void CheckServer(ServerInfoDto serverInfo)
        {
            var iphost = _ipService.GetIpAddress(serverInfo.Host);
            var server = new Arma3BEClient.Steam.Server(new IPEndPoint(IPAddress.Parse(iphost), serverInfo.SteamPort));

            var settings = new GetServerInfoSettings();
            var rules = server.GetServerInfoSync(settings);
            if (string.Compare(rules.Environment, "w", StringComparison.Ordinal) != 0)
            {
                ShowWarning = true;
            }
        }

        public bool ShowWarning
        {
            get => _showWarning;
            set => SetProperty(ref _showWarning, value);
        }

        private CommandType[] NotSupportedCommands = new[]
        {
            CommandType.Mission,
            CommandType.Restart,
            CommandType.RestartServer,
            CommandType.Shutdown
        };


        private bool ConfirmNotSupported(CommandType command)
        {
            if (_showWarning && NotSupportedCommands.Contains(command))
            {
                return MessageBox.Show(Application.Current.MainWindow,
                           $"This command ({command}) may not be supported on server. Are you sure you want to proceed?",
                           "No supported command", MessageBoxButton.YesNo, MessageBoxImage.Warning) ==
                       MessageBoxResult.Yes;
            }
            else
            {
                return MessageBox.Show(Application.Current.MainWindow, "Are you sure?", command.ToString(),
                           MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
            }
        }

        private void SendCommandWithConfirmation(CommandType command)
        {
            if (ConfirmNotSupported(command))
            {
                SendCommand(command);
                MessageBox.Show("Executed " + command, "Server command", MessageBoxButton.OK);
            }
        }

        public string Title => "Manage Server";

        public Mission SelectedMission
        {
            get { return _selectedMission; }
            set
            {
                _selectedMission = value;
                RaisePropertyChanged();
                SetMissionCommand.RaiseCanExecuteChanged();
            }
        }

        public IEnumerable<Mission> Missions
        {
            get { return _missions; }
            set
            {
                _missions = value;
                RaisePropertyChanged();
            }
        }

        public ActionCommand RefreshCommand { get; set; }
        public ActionCommand SetMissionCommand { get; set; }
        public ActionCommand InitCommand { get; set; }
        public ActionCommand ShutdownCommand { get; set; }
        public ActionCommand ReassignCommand { get; set; }
        public ActionCommand RestartCommand { get; set; }
        public ActionCommand RestartServerCommand { get; set; }
        public ActionCommand LockCommand { get; set; }
        public ActionCommand UnlockCommand { get; set; }
        public ActionCommand LoadBansCommand { get; set; }
        public ActionCommand LoadScriptsCommand { get; set; }
        public ActionCommand LoadEventsCommand { get; set; }

        private void SendCommand(CommandType commandType, string parameters = null)
        {
            _eventAggregator.GetEvent<BEMessageEvent<BECommand>>()
                .Publish(new BECommand(_serverId, commandType, parameters));
        }
    }
}