using Arma3BE.Client.Infrastructure.Contracts;
using Arma3BE.Client.Infrastructure.Events;
using Arma3BEClient.Common.Logging;
using Arma3BEClient.Libs.ModelCompact;
using Arma3BEClient.Libs.Repositories;
using Arma3BEClient.Libs.Tools;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Arma3BE.Client.Modules.OptionsModule.ViewModel
{
    public class OptionsModel : DisposableViewModelBase
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly ILog _log = new Log();
        private SettingsStore _settingsStore;

        public OptionsModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            using (var servierInfoRepository = new ServerInfoRepository())
            {
                Servers = servierInfoRepository.GetServerInfo().Select(x => new ServerInfoModel(x)).ToList();
            }

            using (var dc = new ReasonRepository())
            {
                BanReasons = dc.GetBanReasons().Select(x => new ReasonEdit() { Text = x }).ToList();
                KickReasons = dc.GetKickReasons().Select(x => new ReasonEdit() { Text = x }).ToList();
                BanTimes =
                    dc.GetBanTimes().Select(x => new BanTimeEdit() { Text = x.Title, Minutes = x.TimeInMinutes }).ToList();
            }

            var zones = TimeZoneInfo.GetSystemTimeZones().ToArray();
            for (var i = 0; i < zones.Length; i++)
            {
                if (zones[i].Id == Settings.TimeZoneInfo.Id)
                {
                    zones[i] = Settings.TimeZoneInfo;
                }
            }

            TimeZones = zones;
        }

        public List<ServerInfoModel> Servers { get; set; }

        public class ReasonEdit
        {
            public string Text { get; set; }
        }

        public class BanTimeEdit
        {
            public string Text { get; set; }
            public int Minutes { get; set; }
        }

        public List<ReasonEdit> BanReasons { get; set; }
        public List<ReasonEdit> KickReasons { get; set; }
        public List<BanTimeEdit> BanTimes { get; set; }


        public SettingsStore Settings
        {
            get { return _settingsStore ?? (_settingsStore = new SettingsStore() {TimeZoneInfo = SettingsStore.Instance.TimeZoneInfo, AdminName = SettingsStore.Instance.AdminName}); }
            set { _settingsStore = value; }
        }

        public IList<Type> NewListItemTypes
        {
            get { return new List<Type> { typeof(ServerInfoModel) }; }
        }

        public IEnumerable<TimeZoneInfo> TimeZones { get; }

        public void Save()
        {
            try
            {
                var settings = SettingsStore.Instance;
                settings.TimeZoneInfo = Settings.TimeZoneInfo;
                settings.AdminName = Settings.AdminName.Replace(" ", string.Empty);
                settings.Save();

                using (var servierInfoRepository = new ServerInfoRepository())
                {
                    var all = servierInfoRepository.GetServerInfo();

                    var todelete = all.Where(x => Servers.All(s => s.GetDbModel().Id != x.Id));

                    foreach (var serverInfo in todelete)
                    {
                        servierInfoRepository.Remove(serverInfo.Id);
                    }

                    foreach (var s in Servers)
                    {
                        var m = s.GetDbModel();
                        if (m.Id == Guid.Empty)
                        {
                            m.Id = Guid.NewGuid();
                        }
                        servierInfoRepository.AddOrUpdate(m);
                    }
                }

                using (var dc = new ReasonRepository())
                {
                    dc.UpdateBanReasons(BanReasons.Select(x => x.Text).Where(x=>string.IsNullOrEmpty(x) == false).Distinct().ToArray());
                    dc.UpdateBanTimes(BanTimes.Where(x => string.IsNullOrEmpty(x.Text) == false).Select(x => new BanTime() { TimeInMinutes = x.Minutes, Title = x.Text }).ToArray());
                    dc.UpdateKickReasons(KickReasons.Select(x => x.Text).Where(x => string.IsNullOrEmpty(x) == false).Distinct().ToArray());
                }

                _eventAggregator.GetEvent<BEServersChangedEvent>().Publish(null);
            }
            catch (Exception e)
            {
                _log.Error(e);
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    public class ServerInfoModel
    {
        private readonly ServerInfo _info;

        public ServerInfoModel(ServerInfo info)
        {
            _info = info;

            if (_info != null)
            {
                Host = _info.Host;
                Port = _info.Port;
                SteamPort = _info.SteamPort;
                Password = _info.Password;
                Name = _info.Name;
            }
        }

        public ServerInfoModel()
        {
            var model = new ServerInfo();
            model.Id = Guid.Empty;
            _info = model;
        }

        //[Required]
        public string Host
        {
            get { return _info.Host; }
            set { _info.Host = value; }
        }

        //[Required]
        public int Port
        {
            get { return _info.Port; }
            set { _info.Port = value; }
        }

        public int SteamPort
        {
            get { return _info.SteamPort; }
            set { _info.SteamPort = value; }
        }

        //[Required]
        public string Password
        {
            get { return _info.Password; }
            set { _info.Password = value; }
        }

        //[Required]
        public string Name
        {
            get { return _info.Name; }
            set { _info.Name = value; }
        }

        public ServerInfo GetDbModel()
        {
            return _info;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}