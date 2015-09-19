using System;
using System.Collections.Generic;
using System.Windows;
using Arma3BE.Server;
using Arma3BE.Server.Models;
using Arma3BEClient.Commands;
using Arma3BEClient.Common.Logging;
using GalaSoft.MvvmLight;

namespace Arma3BEClient.Models
{
    public class ServerMonitorManageServerViewModel : ViewModelBase
    {
        private readonly ILog _log;
        private readonly Guid _serverId;
        private readonly IBEServer _beServer;
        private IEnumerable<Mission> _missions;
        private Mission _selectedMission;

        public ServerMonitorManageServerViewModel(ILog log, Guid serverId, IBEServer beServer)
        {
            _log = log;
            _serverId = serverId;
            _beServer = beServer;

            _beServer.MissionHandler += BeServerOnMissionHandler;


            SetMissionCommand = new ActionCommand(() =>
            {
                var m = SelectedMission;
                if (m != null)
                {
                    var mn = m.Name;
                    _beServer.SendCommandAsync(CommandType.Mission, mn);
                }
            },
                () => SelectedMission != null);

            RefreshCommand = new ActionCommand(() => _beServer.SendCommandAsync(CommandType.Missions));

            InitCommand = new ActionCommand(async () =>
            {
                await _beServer.SendCommandAsync(CommandType.Init);
                MessageBox.Show("Executed", "Server command", MessageBoxButton.OK);
            });
            ShutdownCommand = new ActionCommand(async () =>
            {
                await _beServer.SendCommandAsync(CommandType.Shutdown);
                MessageBox.Show("Executed", "Server command", MessageBoxButton.OK);
            });
            ReassignCommand = new ActionCommand(async () =>
            {
                await _beServer.SendCommandAsync(CommandType.Reassign);
                MessageBox.Show("Executed", "Server command", MessageBoxButton.OK);
            });
            RestartCommand = new ActionCommand(async () =>
            {
                await _beServer.SendCommandAsync(CommandType.Restart);
                MessageBox.Show("Executed", "Server command", MessageBoxButton.OK);
            });
            LockCommand = new ActionCommand(async () =>
            {
                await _beServer.SendCommandAsync(CommandType.Lock);
                MessageBox.Show("Executed", "Server command", MessageBoxButton.OK);
            });
            UnlockCommand = new ActionCommand(async () =>
            {
                await _beServer.SendCommandAsync(CommandType.Unlock);
                MessageBox.Show("Executed", "Server command", MessageBoxButton.OK);
            });


            LoadBansCommand = new ActionCommand(async () =>
            {
                await _beServer.SendCommandAsync(CommandType.LoadBans);
                MessageBox.Show("Executed", "Server command", MessageBoxButton.OK);
            });
            LoadScriptsCommand = new ActionCommand(async () =>
            {
                await _beServer.SendCommandAsync(CommandType.LoadScripts);
                MessageBox.Show("Executed", "Server command", MessageBoxButton.OK);
            });
            LoadEventsCommand = new ActionCommand(async () =>
            {
                await _beServer.SendCommandAsync(CommandType.LoadEvents);
                MessageBox.Show("Executed", "Server command", MessageBoxButton.OK);
            });
        }

        public Mission SelectedMission
        {
            get { return _selectedMission; }
            set
            {
                _selectedMission = value;
                RaisePropertyChanged("SelectedMission");
                SetMissionCommand.RaiseCanExecuteChanged();
            }
        }

        public IEnumerable<Mission> Missions
        {
            get { return _missions; }
            set
            {
                _missions = value;
                RaisePropertyChanged("Missions");
            }
        }

        public ActionCommand RefreshCommand { get; set; }
        public ActionCommand SetMissionCommand { get; set; }
        public ActionCommand InitCommand { get; set; }
        public ActionCommand ShutdownCommand { get; set; }
        public ActionCommand ReassignCommand { get; set; }
        public ActionCommand RestartCommand { get; set; }
        public ActionCommand LockCommand { get; set; }
        public ActionCommand UnlockCommand { get; set; }
        public ActionCommand LoadBansCommand { get; set; }
        public ActionCommand LoadScriptsCommand { get; set; }
        public ActionCommand LoadEventsCommand { get; set; }

        private void BeServerOnMissionHandler(object sender, UpdateClientEventArgs<IEnumerable<Mission>> e)
        {
            Missions = e.Data;
        }
    }
}