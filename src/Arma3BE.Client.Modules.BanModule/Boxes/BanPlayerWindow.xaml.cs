using Arma3BE.Client.Infrastructure.Models;
using Arma3BE.Client.Modules.BanModule.Helpers;
using Arma3BE.Server.Abstract;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Windows;

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
        private readonly BanHelper _playerHelper;
        private readonly string _playerNum;
        private long? _minutes;
        private readonly Guid _serverId;
        private string _playerGuid;
        private string _playerName;
        private string _reason;
        private TimeSpan _timeSpan;

        public BanPlayerViewModel(Guid serverId, string playerGuid, bool isOnline, BanHelper playerHelper, string playerName,
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
                    var str =
                        ConfigurationManager.AppSettings["Ban_reasons"].Split('|')
                            .Where(x => !string.IsNullOrEmpty(x))
                            .Select(x => x.Trim())
                            .ToArray();

                    return str;
                }
                catch (Exception)
                {
                    return new[] { string.Empty };
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

        public TimeSpan TimeSpan
        {
            get { return _timeSpan; }
            set
            {
                _timeSpan = value;
                OnPropertyChanged();
                Minutes = (long)_timeSpan.TotalMinutes;
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
    }
}