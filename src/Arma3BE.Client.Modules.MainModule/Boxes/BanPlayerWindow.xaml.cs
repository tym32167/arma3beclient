using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Windows;
using Arma3BE.Client.Modules.MainModule.Helpers;
using GalaSoft.MvvmLight;

namespace Arma3BE.Client.Modules.MainModule.Boxes
{
    /// <summary>
    ///     Interaction logic for BanPlayerWindow.xaml
    /// </summary>
    public partial class BanPlayerWindow : Window
    {
        private readonly BanPlayerViewModel _model;

        public BanPlayerWindow(PlayerHelper playerHelper, string playerGuid, bool isOnline, string playerName,
            string playerNum)
        {
            InitializeComponent();
            _model = new BanPlayerViewModel(playerGuid, isOnline, playerHelper, playerName, playerNum);

            tbGuid.IsEnabled = string.IsNullOrEmpty(playerGuid);

            var tsdata = new List<TimeSpan>
            {
                TimeSpan.FromDays(0),
                TimeSpan.FromDays(1),
                TimeSpan.FromDays(7),
                TimeSpan.FromDays(30)
            };

            ts.ItemsSource = tsdata;

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
        private readonly PlayerHelper _playerHelper;
        private readonly string _playerNum;
        private long? _minutes;
        private string _playerGuid;
        private string _playerName;
        private string _reason;
        private TimeSpan _timeSpan;

        public BanPlayerViewModel(string playerGuid, bool isOnline, PlayerHelper playerHelper, string playerName,
            string playerNum)
        {
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
                RaisePropertyChanged("PlayerName");
            }
        }

        public IEnumerable<string> Reasons
        {
            get
            {
                try
                {
                    var str =
                        ConfigurationManager.AppSettings["Ban_reasons"].Split('|')
                            .Where(x => !string.IsNullOrEmpty(x))
                            .Select(x => x.Trim())
                            .ToArray();

                    return str;
                }
                catch (Exception)
                {
                    return new[] {string.Empty};
                }
            }
        }

        public string PlayerGuid
        {
            get { return _playerGuid; }
            set
            {
                _playerGuid = value;
                RaisePropertyChanged("PlayerGuid");
            }
        }

        public string Reason
        {
            get { return _reason; }
            set
            {
                _reason = value;
                RaisePropertyChanged("Reason");
            }
        }

        public long? Minutes
        {
            get { return _minutes; }
            set
            {
                _minutes = value;
                RaisePropertyChanged("Minutes");
            }
        }

        public TimeSpan TimeSpan
        {
            get { return _timeSpan; }
            set
            {
                _timeSpan = value;
                RaisePropertyChanged("TimeSpan");
                Minutes = (long) _timeSpan.TotalMinutes;
            }
        }

        public void Ban()
        {
            if (Minutes != null)
            {
                if (_isOnline)
                    _playerHelper.BanGuidOnline(_playerNum, _playerGuid, Reason, Minutes.Value);
                else
                    _playerHelper.BanGUIDOffline(_playerGuid, Reason, Minutes.Value);
            }
        }
    }
}