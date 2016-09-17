using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Arma3BE.Client.Infrastructure.Commands;
using Arma3BE.Client.Infrastructure.Models;
using Arma3BEClient.Common.Core;
using Arma3BEClient.Libs.Repositories;

namespace Arma3BE.Client.Modules.SteamModule.Models
{
    public class SteamDiscoveryViewModel : DisposableViewModelBase, ITitledItem
    {
        private CancellationTokenSource _cancelatioTokenSource;
        private long _current;
        
        private bool _isBusy;
        private ObservableCollection<Tuple<string, string>> _playersFound;
        private int _progress;
        private HashSet<string> _totalPlayers;
        private long _min;
        private long _max;
        private bool _inProcess;
        

        public SteamDiscoveryViewModel()
        {
            PlayersFound = new ObservableCollection<Tuple<string, string>>();
            StartCommand = new ActionCommand(Start);
            StopCommand = new ActionCommand(Stop);

            Max = 8274000000L;
            Min = 7959000000L;

            Current = Min;
            
        }

        protected override void DisposeManagedResources()
        {
            base.DisposeManagedResources();
            _cancelatioTokenSource?.Cancel();
        }

        
        public static string StaticTitle = "Steam Discovery";

        public string Title => StaticTitle;

        public ICommand StartCommand { get; set; }
        public ICommand StopCommand { get; set; }

        public long Min
        {
            get { return _min; }
            set
            {
                _min = value;
                OnPropertyChanged();
            }
        }

        public long Max
        {
            get { return _max; }
            set
            {
                _max = value;
                OnPropertyChanged();
            }
        }

        public long Current
        {
            get { return _current; }
            set
            {
                _current = value;
                OnPropertyChanged();
            }
        }

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                OnPropertyChanged();
            }
        }

        public bool InProcess
        {
            get { return _inProcess; }
            set
            {
                _inProcess = value;
                OnPropertyChanged();
            }
        }

        public HashSet<string> TotalPlayers
        {
            get { return _totalPlayers; }
            set
            {
                _totalPlayers = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Tuple<string, string>> PlayersFound
        {
            get { return _playersFound; }
            set
            {
                _playersFound = value;
                OnPropertyChanged();
            }
        }

        public int Progress
        {
            get { return _progress; }
            set
            {
                if (_progress != value)
                {
                    _progress = value;
                    OnPropertyChanged();
                }
            }
        }

        private void Stop()
        {
            IsBusy = true;

            Task.Factory.StartNew(() =>
            {
                try
                {
                    _cancelatioTokenSource?.Cancel();
                    Save();
                }
                finally
                {
                    IsBusy = false;
                    InProcess = false;
                }
            });
        }

        private void Save()
        {
            var found = PlayersFound;
            if (found != null && found.Count > 0)
            {
                using (var repo = PlayerRepositoryFactory.Create())
                {
                    var data = found.GroupBy(x => x.Item1, x => x.Item2).ToDictionary(x => x.Key, x => x.First());
                    repo.SaveSteamId(data);
                }
            }

            InProcess = false;
        }


        private void Start()
        {
            InProcess = true;
            var found = PlayersFound;
            found.Clear();

            if (TotalPlayers == null)
            {
                using (var repo = PlayerRepositoryFactory.Create())
                {
                    TotalPlayers = repo.GetAllGuidsWithoutSteam();
                }
            }

            _cancelatioTokenSource?.Cancel();
            _cancelatioTokenSource = new CancellationTokenSource();

            Task.Factory.StartNew(() => GenerateStart(_cancelatioTokenSource.Token), TaskCreationOptions.LongRunning);
        }

        private void GenerateStart(CancellationToken token)
        {
            Generate(Min, Max, token);
        }

        private void Generate(long min, long max, CancellationToken token)
        {
            var start = Current;

            for (var i = start; i <= max; i++)
            {
                var l = 76561190000000000L + i;
                var hash = Steam_Hash(l);
                Progress = (int)((i - min) * 100.0 / (max - min));

                CheckHash(l.ToString(), hash);

                Current = i;
                if (token.IsCancellationRequested)
                {
                    return;
                }
            }

            Save();
        }

        private void CheckHash(string currentId, string hash)
        {
            if (TotalPlayers?.Contains(hash) == true)
            {
                PlayersFound.Add(new Tuple<string, string>(hash, currentId));
            }
        }

        private static string Steam_Hash(long num)
        {
            // http://steamcommunity.com/profiles/7656119xxxxxxxxxx 
            // http://steamcommunity.com/profiles/76561198053877632
            var steamId = num;
            var parts = new byte[] { 0x42, 0x45, 0, 0, 0, 0, 0, 0, 0, 0 };

            for (var i = 2; i < 10; i++)
            {
                var res = steamId % 256;
                steamId = steamId / 256;
                parts[i] = (byte)res;
            }

            var md5 = new MD5CryptoServiceProvider();
            var hash = md5.ComputeHash(parts);

            var result = "";
            foreach (var b in hash)
            {
                result += b.ToString("x2");
            }

            return result;
        }
    }
}