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
        private readonly UpdateClient _updateClient;
        private IEnumerable<Mission> _missions;
        private Mission _selectedMission;

        public ServerMonitorManageServerViewModel(ILog log, Guid serverId, UpdateClient updateClient)
        {
            _log = log;
            _serverId = serverId;
            _updateClient = updateClient;

            _updateClient.MissionHandler += UpdateClientOnMissionHandler;


            SetMissionCommand = new ActionCommand(() =>
            {
                var m = SelectedMission;
                if (m != null)
                {
                    var mn = m.Name;
                    _updateClient.SendCommandAsync(UpdateClient.CommandType.Mission, mn);
                }
            },
                () => SelectedMission != null);

            RefreshCommand = new ActionCommand(() => _updateClient.SendCommandAsync(UpdateClient.CommandType.Missions));

            InitCommand = new ActionCommand(async () =>
            {
                await _updateClient.SendCommandAsync(UpdateClient.CommandType.Init);
                MessageBox.Show("Executed", "Server command", MessageBoxButton.OK);
            });
            ShutdownCommand = new ActionCommand(async () =>
            {
                await _updateClient.SendCommandAsync(UpdateClient.CommandType.Shutdown);
                MessageBox.Show("Executed", "Server command", MessageBoxButton.OK);
            });
            ReassignCommand = new ActionCommand(async () =>
            {
                await _updateClient.SendCommandAsync(UpdateClient.CommandType.Reassign);
                MessageBox.Show("Executed", "Server command", MessageBoxButton.OK);
            });
            RestartCommand = new ActionCommand(async () =>
            {
                await _updateClient.SendCommandAsync(UpdateClient.CommandType.Restart);
                MessageBox.Show("Executed", "Server command", MessageBoxButton.OK);
            });
            LockCommand = new ActionCommand(async () =>
            {
                await _updateClient.SendCommandAsync(UpdateClient.CommandType.Lock);
                MessageBox.Show("Executed", "Server command", MessageBoxButton.OK);
            });
            UnlockCommand = new ActionCommand(async () =>
            {
                await _updateClient.SendCommandAsync(UpdateClient.CommandType.Unlock);
                MessageBox.Show("Executed", "Server command", MessageBoxButton.OK);
            });


            LoadBansCommand = new ActionCommand(async () =>
            {
                await _updateClient.SendCommandAsync(UpdateClient.CommandType.LoadBans);
                MessageBox.Show("Executed", "Server command", MessageBoxButton.OK);
            });
            LoadScriptsCommand = new ActionCommand(async () =>
            {
                await _updateClient.SendCommandAsync(UpdateClient.CommandType.LoadScripts);
                MessageBox.Show("Executed", "Server command", MessageBoxButton.OK);
            });
            LoadEventsCommand = new ActionCommand(async () =>
            {
                await _updateClient.SendCommandAsync(UpdateClient.CommandType.LoadEvents);
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

        private void UpdateClientOnMissionHandler(object sender, UpdateClientEventArgs<IEnumerable<Mission>> e)
        {
            Missions = e.Data;
        }
    }
}