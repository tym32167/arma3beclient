using Arma3BE.Client.Infrastructure.Helpers;
using Arma3BE.Client.Infrastructure.Models;
using Arma3BEClient.Libs.Repositories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
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
            string playerNum, IServerInfoRepository infoRepository)
        {
            InitializeComponent();
            _model = new BanPlayerViewModel(serverId, playerGuid, isOnline, banHelper, playerName, playerNum, infoRepository);

            tbGuid.IsEnabled = string.IsNullOrEmpty(playerGuid);

            DataContext = _model;
        }

        private async void BanClick(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_model.Reason) && lnMinutes.Value.HasValue)
            {
                await _model.BanAsync();
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
            string playerNum, IServerInfoRepository infoRepository)
        {
            _playerGuid = playerGuid;
            _isOnline = isOnline;
            _playerHelper = playerHelper;
            _playerName = playerName;
            _playerNum = playerNum;
            _minutes = 0;


            Init(infoRepository, serverId);
        }

        private async Task Init(IServerInfoRepository infoRepository, Guid? serverId)
        {
            Servers = (await infoRepository.GetActiveServerInfoAsync()).OrderBy(x => x.Name).ToList();


            if (string.IsNullOrEmpty(_playerName))
                using (var userRepo = new PlayerRepository())
                {
                    var player = await userRepo.GetPlayerAsync(_playerGuid);
                    _playerName = player?.Name;
                }

            SelectedServers = new ObservableCollection<ServerInfoDto>();

            if (serverId.HasValue)
                SelectedServers.AddRange(Servers.Where(s => s.Id == serverId.Value));

            Reasons = await GetReasons();
            BanTimes = await GetBanTimes();

            RaisePropertyChanged(nameof(Reasons));
            RaisePropertyChanged(nameof(BanTimes));
        }

        public List<ServerInfoDto> Servers { get; private set; }

        public ObservableCollection<ServerInfoDto> SelectedServers { get; private set; }

        public string PlayerName
        {
            get { return _playerName; }
            set
            {
                _playerName = value;
                RaisePropertyChanged();
            }
        }

        async Task<string[]> GetReasons()
        {
            try
            {
                using (var repo = new ReasonRepository())
                {
                    var str = await repo.GetBanReasonsAsync();
                    return str;
                }
            }
            catch (Exception)
            {
                return new[] { string.Empty };
            }
        }

        public IEnumerable<string> Reasons { get; set; }


        async Task<IEnumerable<BanFullTime>> GetBanTimes()
        {
            try
            {
                using (var repo = new ReasonRepository())
                {
                    var str = (await repo.GetBanTimesAsync()).Select(x => new BanFullTime
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

        public IEnumerable<BanFullTime> BanTimes { get; set; }


        public string PlayerGuid
        {
            get { return _playerGuid; }
            set
            {
                _playerGuid = value;
                RaisePropertyChanged();
            }
        }

        public string Reason
        {
            get { return _reason; }
            set
            {
                _reason = value;
                RaisePropertyChanged();
            }
        }

        public long? Minutes
        {
            get { return _minutes; }
            set
            {
                _minutes = value;
                RaisePropertyChanged();
            }
        }

        public BanFullTime TimeSpan
        {
            get { return _timeSpan; }
            set
            {
                _timeSpan = value;
                RaisePropertyChanged();
                Minutes = _timeSpan.PeriodMinutes;
            }
        }

        public async Task BanAsync()
        {
            if (Minutes != null)
            {
                foreach (var selectedServer in SelectedServers)
                {
                    if (_isOnline)
                        await _playerHelper.BanGuidOnlineAsync(selectedServer.Id, _playerNum, _playerGuid, Reason, Minutes.Value);
                    else
                        await _playerHelper.BanGUIDOfflineAsync(selectedServer.Id, _playerGuid, Reason, Minutes.Value);
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