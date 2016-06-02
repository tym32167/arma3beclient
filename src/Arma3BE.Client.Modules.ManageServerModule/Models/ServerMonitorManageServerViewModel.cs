using Arma3BE.Client.Infrastructure.Commands;
using Arma3BE.Client.Infrastructure.Models;
using Arma3BE.Server;
using Arma3BE.Server.Abstract;
using Arma3BE.Server.Models;
using Arma3BEClient.Common.Logging;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Arma3BE.Client.Modules.ManageServerModule.Models
{
    public class ServerMonitorManageServerViewModel : ViewModelBase, IServerMonitorManageServerViewModel
    {
        private readonly IBEServer _beServer;
        private readonly ILog _log;
        private readonly Guid _serverId;
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
                    _beServer.SendCommand(CommandType.Mission, mn);
                }
            },
                () => SelectedMission != null);

            RefreshCommand = new ActionCommand(() => _beServer.SendCommand(CommandType.Missions));

            InitCommand = new ActionCommand(() =>
           {
               _beServer.SendCommand(CommandType.Init);
               MessageBox.Show("Executed", "Server command", MessageBoxButton.OK);
           });
            ShutdownCommand = new ActionCommand(() =>
           {
               _beServer.SendCommand(CommandType.Shutdown);
               MessageBox.Show("Executed", "Server command", MessageBoxButton.OK);
           });
            ReassignCommand = new ActionCommand(() =>
           {
               _beServer.SendCommand(CommandType.Reassign);
               MessageBox.Show("Executed", "Server command", MessageBoxButton.OK);
           });
            RestartCommand = new ActionCommand(() =>
           {
               _beServer.SendCommand(CommandType.Restart);
               MessageBox.Show("Executed", "Server command", MessageBoxButton.OK);
           });
            LockCommand = new ActionCommand(() =>
           {
               _beServer.SendCommand(CommandType.Lock);
               MessageBox.Show("Executed", "Server command", MessageBoxButton.OK);
           });
            UnlockCommand = new ActionCommand(() =>
           {
               _beServer.SendCommand(CommandType.Unlock);
               MessageBox.Show("Executed", "Server command", MessageBoxButton.OK);
           });


            LoadBansCommand = new ActionCommand(() =>
           {
               _beServer.SendCommand(CommandType.LoadBans);
               MessageBox.Show("Executed", "Server command", MessageBoxButton.OK);
           });
            LoadScriptsCommand = new ActionCommand(() =>
           {
               _beServer.SendCommand(CommandType.LoadScripts);
               MessageBox.Show("Executed", "Server command", MessageBoxButton.OK);
           });
            LoadEventsCommand = new ActionCommand(() =>
           {
               _beServer.SendCommand(CommandType.LoadEvents);
               MessageBox.Show("Executed", "Server command", MessageBoxButton.OK);
           });
        }

        public Mission SelectedMission
        {
            get { return _selectedMission; }
            set
            {
                _selectedMission = value;
                OnPropertyChanged("SelectedMission");
                SetMissionCommand.RaiseCanExecuteChanged();
            }
        }

        public IEnumerable<Mission> Missions
        {
            get { return _missions; }
            set
            {
                _missions = value;
                OnPropertyChanged("Missions");
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