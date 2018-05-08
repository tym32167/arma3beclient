﻿using Arma3BE.Client.Infrastructure.Contracts;
using Arma3BE.Client.Infrastructure.Events;
using Arma3BE.Client.Modules.CoreModule.Helpers;
using Arma3BEClient.Common.Logging;
using Arma3BEClient.Libs.Core;
using Arma3BEClient.Libs.Core.Settings;
using Arma3BEClient.Libs.EF.Model;
using Arma3BEClient.Libs.EF.Repositories;
using Microsoft.Practices.ServiceLocation;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Arma3BEClient.Libs.Core.Model;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable ExplicitCallerInfoArgument

namespace Arma3BE.Client.Modules.OptionsModule.ViewModel
{
    public class OptionsModel : DisposableViewModelBase
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly ISettingsStoreSource _settingsStoreSource;
        private readonly MessageHelper _messageHelper;
        private readonly IServerInfoRepository _infoRepository;
        private readonly ILog _log = new Log();
        private ISettingsStore _settingsStore;

        public OptionsModel(IEventAggregator eventAggregator, ISettingsStoreSource settingsStoreSource, MessageHelper messageHelper, IServerInfoRepository infoRepository)
        {
            _eventAggregator = eventAggregator;
            _settingsStoreSource = settingsStoreSource;
            _messageHelper = messageHelper;
            _infoRepository = infoRepository;

            Init();
        }

        private async void Init()
        {
            Servers = (await _infoRepository.GetServerInfoAsync()).Select(x => new ServerInfoModel(x)).ToList();


            using (var dc = ServiceLocator.Current.TryResolve<IReasonRepository>())
            {
                BanReasons = (await dc.GetBanReasonsAsync()).Select(x => new ReasonEdit { Text = x }).ToList();
                KickReasons = (await dc.GetKickReasonsAsync()).Select(x => new ReasonEdit { Text = x }).ToList();
                BanTimes =
                    (await dc.GetBanTimesAsync()).Select(x => new BanTimeEdit { Text = x.Title, Minutes = x.TimeInMinutes }).ToList();

                BadNicknames = (await dc.GetBadNicknamesAsync()).Select(x => new ReasonEdit { Text = x }).ToList();
                ImportantWords = (await dc.GetImportantWordsAsync()).Select(x => new ReasonEdit { Text = x }).ToList();
            }

            var zones = TimeZoneInfo.GetSystemTimeZones().ToArray();
            for (var i = 0; i < zones.Length; i++)
            {
                if (zones[i].Id == Settings.TimeZoneInfo?.Id)
                {
                    zones[i] = Settings.TimeZoneInfo;
                }
            }

            TimeZones = zones;

            RaisePropertyChanged(nameof(Servers));
            RaisePropertyChanged(nameof(BanReasons));
            RaisePropertyChanged(nameof(KickReasons));
            RaisePropertyChanged(nameof(BanTimes));
            RaisePropertyChanged(nameof(BadNicknames));
            RaisePropertyChanged(nameof(ImportantWords));
            RaisePropertyChanged(nameof(TimeZones));
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

        public List<ReasonEdit> BadNicknames { get; set; }
        public List<ReasonEdit> ImportantWords { get; set; }


        public ISettingsStore Settings
        {
            get
            {
                return _settingsStore ?? (_settingsStore = _settingsStoreSource.GetSettingsStore().Clone() as ISettingsStore);
            }
            set { _settingsStore = value; }
        }

        public IList<Type> NewListItemTypes => new List<Type> { typeof(ServerInfoModel) };


        public string BanMessageTemplateExample => $"{_messageHelper.GetBanMessage(Settings, "Sample reason", 0)}\n{_messageHelper.GetBanMessage(Settings, "Sample reason", 10)}";


        public string BanMessageTemplate
        {
            get { return Settings.BanMessageTemplate; }
            set
            {
                Settings.BanMessageTemplate = value;
                RaisePropertyChanged(nameof(BanMessageTemplate));
                RaisePropertyChanged(nameof(BanMessageTemplateExample));
            }
        }


        public string KickMessageTemplateExample => _messageHelper.GetKickMessage(Settings, "Sample reason");


        public string KickMessageTemplate
        {
            get { return Settings.KickMessageTemplate; }
            set
            {
                Settings.KickMessageTemplate = value;
                RaisePropertyChanged(nameof(KickMessageTemplate));
                RaisePropertyChanged(nameof(KickMessageTemplateExample));
            }
        }


        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public IEnumerable<TimeZoneInfo> TimeZones { get; set; }

        public async Task Save()
        {
            try
            {
                var settings = _settingsStoreSource.GetSettingsStore();
                settings.TimeZoneInfo = Settings.TimeZoneInfo;

                settings.AdminName = Settings.AdminName.Replace(" ", string.Empty);

                settings.BanMessageTemplate = Settings.BanMessageTemplate;
                settings.KickMessageTemplate = Settings.KickMessageTemplate;


                settings.BansUpdateSeconds = Settings.BansUpdateSeconds;
                settings.PlayersUpdateSeconds = Settings.PlayersUpdateSeconds;

                settings.TopMost = Settings.TopMost;

                settings.Save();


                var all = await _infoRepository.GetServerInfoAsync();

                var todelete = all.Where(x => Servers.All(s => s.GetDbModel().Id != x.Id));

                foreach (var serverInfo in todelete)
                {
                    await _infoRepository.RemoveAsync(serverInfo.Id);
                }

                foreach (var s in Servers)
                {
                    var m = s.GetDbModel();
                    if (m.Id == Guid.Empty)
                    {
                        m.Id = Guid.NewGuid();
                    }
                    await _infoRepository.AddOrUpdateAsync(m);
                }


                using (var dc = ServiceLocator.Current.TryResolve<IReasonRepository>())
                {
                    await dc.UpdateBanReasons(BanReasons.Select(x => x.Text).Where(x => string.IsNullOrEmpty(x) == false).Distinct().ToArray());
                    await dc.UpdateBanTimes(BanTimes.Where(x => string.IsNullOrEmpty(x.Text) == false).Select(x => new BanTime { TimeInMinutes = x.Minutes, Title = x.Text }).ToArray());
                    await dc.UpdateKickReasons(KickReasons.Select(x => x.Text).Where(x => string.IsNullOrEmpty(x) == false).Distinct().ToArray());


                    await dc.UpdateBadNicknames(BadNicknames.Select(x => x.Text).Where(x => string.IsNullOrEmpty(x) == false).Distinct().ToArray());
                    await dc.UpdateImportantWords(ImportantWords.Select(x => x.Text).Where(x => string.IsNullOrEmpty(x) == false).Distinct().ToArray());
                }

                _eventAggregator.GetEvent<BEServersChangedEvent>().Publish(null);
                _eventAggregator.GetEvent<SettingsChangedEvent>().Publish(_settingsStoreSource.GetSettingsStore());


                foreach (Window wnd in Application.Current.Windows) wnd.Topmost = settings.TopMost;
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
        private readonly ServerInfoDto _info;

        public ServerInfoModel(ServerInfoDto info)
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
            var model = new ServerInfoDto();
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

        public ServerInfoDto GetDbModel()
        {
            return _info;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}