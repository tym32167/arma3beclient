using Arma3BE.Client.Infrastructure.Commands;
using Arma3BE.Client.Infrastructure.Events.BE;
using Arma3BE.Client.Infrastructure.Models;
using Arma3BE.Server;
using Arma3BE.Server.Models;
using Arma3BEClient.Libs.ModelCompact;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Windows;
using Arma3BEClient.Libs.Repositories;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Arma3BE.Client.Modules.ManageServerModule.Models
{
    public class ServerMonitorManageServerViewModel : ViewModelBase
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly Guid _serverId;
        private IEnumerable<Mission> _missions;
        private Mission _selectedMission;

        public ServerMonitorManageServerViewModel(ServerInfoDto serverInfo, IEventAggregator eventAggregator)
        {
            _serverId = serverInfo.Id;
            _eventAggregator = eventAggregator;

            _eventAggregator.GetEvent<BEMessageEvent<BEItemsMessage<Mission>>>().Subscribe(e =>
            {
                if (e.ServerId == _serverId)
                    Missions = e.Items;
            });

            SetMissionCommand = new ActionCommand(() =>
                {
                    var m = SelectedMission;
                    if (m != null)
                    {
                        var mn = m.Name;
                        SendCommand(CommandType.Mission, mn);
                    }
                },
                () => SelectedMission != null);

            RefreshCommand = new ActionCommand(() => SendCommand(CommandType.Missions));

            InitCommand = new ActionCommand(() =>
            {
                SendCommandWithConfirmation(CommandType.Init);
            });
            ShutdownCommand = new ActionCommand(() =>
            {
                SendCommandWithConfirmation(CommandType.Shutdown);
            });
            ReassignCommand = new ActionCommand(() =>
            {
                SendCommandWithConfirmation(CommandType.Reassign);
            });
            RestartCommand = new ActionCommand(() =>
            {
                SendCommandWithConfirmation(CommandType.Restart);
            });

            RestartServerCommand = new ActionCommand(() =>
            {
                SendCommandWithConfirmation(CommandType.RestartServer);
            });

            LockCommand = new ActionCommand(() =>
            {
                SendCommandWithConfirmation(CommandType.Lock);
            });
            UnlockCommand = new ActionCommand(() =>
            {
                SendCommandWithConfirmation(CommandType.Unlock);
            });

            LoadBansCommand = new ActionCommand(() =>
            {
                SendCommandWithConfirmation(CommandType.LoadBans);
            });
            LoadScriptsCommand = new ActionCommand(() =>
            {
                SendCommandWithConfirmation(CommandType.LoadScripts);
            });
            LoadEventsCommand = new ActionCommand(() =>
            {
                SendCommandWithConfirmation(CommandType.LoadEvents);
            });
        }


        private void SendCommandWithConfirmation(CommandType command)
        {
            if (MessageBox.Show(Application.Current.MainWindow, "Are you sure?", command.ToString(),
                       MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
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
                OnPropertyChanged();
                SetMissionCommand.RaiseCanExecuteChanged();
            }
        }

        public IEnumerable<Mission> Missions
        {
            get { return _missions; }
            set
            {
                _missions = value;
                OnPropertyChanged();
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