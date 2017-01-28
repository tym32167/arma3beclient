using Arma3BE.Client.Infrastructure.Helpers;
using Arma3BE.Client.Infrastructure.Models;
using Arma3BEClient.Libs.ModelCompact;
using Arma3BEClient.Libs.Repositories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
// ReSharper disable MemberCanBePrivate.Global

namespace Arma3BE.Client.Modules.BanModule.Boxes
{
    /// <summary>
    ///     Interaction logic for BanPlayerWindow.xaml
    /// </summary>
    // ReSharper disable once RedundantExtendsListEntry
    public partial class BanPlayerWindow : Window
    {
        private readonly BanPlayerViewModel _model;

        public BanPlayerWindow(Guid? serverId, IBanHelper banHelper, string playerGuid, bool isOnline, string playerName,
            string playerNum)
        {
            InitializeComponent();
            _model = new BanPlayerViewModel(serverId, playerGuid, isOnline, banHelper, playerName, playerNum);

            tbGuid.IsEnabled = string.IsNullOrEmpty(playerGuid);

            DataContext = _model;
        }

        private void BanClick(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_model.Reason) && lnMinutes.Value.HasValue)
            {
                _model.Ban();
                Close();
            }
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }

    public class BanPlayerViewModel : ViewModelBase
    {
        private readonly bool _isOnline;
        private readonly IBanHelper _playerHelper;
        private readonly string _playerNum;
        private long? _minutes;
        private string _playerGuid;
        private string _playerName;
        private string _reason;
        private BanFullTime _timeSpan;

        public BanPlayerViewModel(Guid? serverId, string playerGuid, bool isOnline, IBanHelper playerHelper,
            string playerName,
            string playerNum)
        {
            _playerGuid = playerGuid;
            _isOnline = isOnline;
            _playerHelper = playerHelper;
            _playerName = playerName;
            _playerNum = playerNum;
            _minutes = 0;

            using (var repo = new ServerInfoRepository())
                Servers = repo.GetActiveServerInfo().OrderBy(x => x.Name).ToList();


            if (string.IsNullOrEmpty(playerName))
                using (var userRepo = new PlayerRepository())
                {
                    var player = userRepo.GetPlayer(playerGuid);
                    _playerName = player?.Name;
                }

            SelectedServers = new ObservableCollection<ServerInfo>();

            if (serverId.HasValue)
                SelectedServers.AddRange(Servers.Where(s => s.Id == serverId.Value));
        }

        public List<ServerInfo> Servers { get; }

        public ObservableCollection<ServerInfo> SelectedServers { get; }

        public string PlayerName
        {
            get { return _playerName; }
            set
            {
                _playerName = value;
                OnPropertyChanged();
            }
        }

        public IEnumerable<string> Reasons
        {
            get
            {
                try
                {
                    using (var repo = new ReasonRepository())
                    {
                        var str = repo.GetBanReasons();
                        return str;
                    }
                }
                catch (Exception)
                {
                    return new[] { string.Empty };
                }
            }
        }

        public IEnumerable<BanFullTime> BanTimes
        {
            get
            {
                try
                {
                    using (var repo = new ReasonRepository())
                    {
                        var str = repo.GetBanTimes().Select(x => new BanFullTime
                        {
                            Text = x.Title,
                            Period = System.TimeSpan.FromMinutes(x.TimeInMinutes),
                            PeriodMinutes = x.TimeInMinutes
                        }).ToArray();
                        return str;
                    }
                }
                catch (Exception)
                {
                    return Enumerable.Empty<BanFullTime>();
                }
            }
        }


        public string PlayerGuid
        {
            get { return _playerGuid; }
            set
            {
                _playerGuid = value;
                OnPropertyChanged();
            }
        }

        public string Reason
        {
            get { return _reason; }
            set
            {
                _reason = value;
                OnPropertyChanged();
            }
        }

        public long? Minutes
        {
            get { return _minutes; }
            set
            {
                _minutes = value;
                OnPropertyChanged();
            }
        }

        public BanFullTime TimeSpan
        {
            get { return _timeSpan; }
            set
            {
                _timeSpan = value;
                OnPropertyChanged();
                Minutes = _timeSpan.PeriodMinutes;
            }
        }

        public void Ban()
        {
            if (Minutes != null)
            {
                foreach (var selectedServer in SelectedServers)
                {
                    if (_isOnline)
                        _playerHelper.BanGuidOnline(selectedServer.Id, _playerNum, _playerGuid, Reason, Minutes.Value);
                    else
                        _playerHelper.BanGUIDOffline(selectedServer.Id, _playerGuid, Reason, Minutes.Value);
                }
            }
        }

        public class BanFullTime
        {
            public string Text { get; set; }
            public TimeSpan Period { get; set; }
            public int PeriodMinutes { get; set; }

            public string Display => $"{Text} ({Period})";
        }
    }
}