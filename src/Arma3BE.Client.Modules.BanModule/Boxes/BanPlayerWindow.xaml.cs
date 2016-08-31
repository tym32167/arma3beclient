using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Arma3BE.Client.Infrastructure.Models;
using Arma3BE.Client.Modules.BanModule.Helpers;
using Arma3BEClient.Libs.Repositories;

namespace Arma3BE.Client.Modules.BanModule.Boxes
{
    /// <summary>
    ///     Interaction logic for BanPlayerWindow.xaml
    /// </summary>
    public partial class BanPlayerWindow : Window
    {
        private readonly BanPlayerViewModel _model;

        public BanPlayerWindow(Guid serverId, BanHelper banHelper, string playerGuid, bool isOnline, string playerName,
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
        private readonly BanHelper _playerHelper;
        private readonly string _playerNum;
        private readonly Guid _serverId;
        private long? _minutes;
        private string _playerGuid;
        private string _playerName;
        private string _reason;
        private BanFullTime _timeSpan;

        public BanPlayerViewModel(Guid serverId, string playerGuid, bool isOnline, BanHelper playerHelper,
            string playerName,
            string playerNum)
        {
            _serverId = serverId;
            _playerGuid = playerGuid;
            _isOnline = isOnline;
            _playerHelper = playerHelper;
            _playerName = playerName;
            _playerNum = playerNum;
            _minutes = 0;
        }

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
                if (_isOnline)
                    _playerHelper.BanGuidOnline(_serverId, _playerNum, _playerGuid, Reason, Minutes.Value);
                else
                    _playerHelper.BanGUIDOffline(_serverId, _playerGuid, Reason, Minutes.Value);
            }
        }

        public class BanFullTime
        {
            public string Text { get; set; }
            public TimeSpan Period { get; set; }
            public int PeriodMinutes { get; set; }

            public string Display
            {
                get { return $"{Text} ({Period})"; }
            }
        }
    }
}