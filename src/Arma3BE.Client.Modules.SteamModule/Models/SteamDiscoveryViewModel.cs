using Arma3BE.Client.Infrastructure.Commands;
using Arma3BE.Client.Infrastructure.Models;
using Arma3BEClient.Libs.Repositories.Players;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Arma3BE.Client.Modules.SteamModule.Models
{
    public class SteamDiscoveryViewModel : DisposableViewModelBase, ITitledItem
    {
        public static string StaticTitle = "Steam Discovery";
        private readonly IPlayerRepository _playerRepository;
        private CancellationTokenSource _cancelatioTokenSource;
        private long _current;
        private bool _inProcess;

        private bool _isBusy;
        private long _max;
        private long _min;
        private ObservableCollection<Tuple<string, string>> _playersFound;
        private int _progress;
        private Dictionary<string, Guid> _totalPlayers;


        public SteamDiscoveryViewModel(IPlayerRepository playerRepository)
        {
            _playerRepository = playerRepository;
            PlayersFound = new ObservableCollection<Tuple<string, string>>();
            StartCommand = new ActionCommand(async () => await StartAsync());
            StopCommand = new ActionCommand(Stop);

            Max = 8274000000L;
            Min = 7959000000L;

            Current = Min;
        }

        public ICommand StartCommand { get; set; }
        public ICommand StopCommand { get; set; }

        public long Min
        {
            get { return _min; }
            set
            {
                _min = value;
                RaisePropertyChanged();
            }
        }

        public long Max
        {
            get { return _max; }
            set
            {
                _max = value;
                RaisePropertyChanged();
            }
        }

        public long Current
        {
            get { return _current; }
            set
            {
                _current = value;
                RaisePropertyChanged();
            }
        }

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                RaisePropertyChanged();
            }
        }

        public bool InProcess
        {
            get { return _inProcess; }
            set
            {
                _inProcess = value;
                RaisePropertyChanged();
            }
        }

        public Dictionary<string, Guid> TotalPlayers
        {
            get { return _totalPlayers; }
            set
            {
                _totalPlayers = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<Tuple<string, string>> PlayersFound
        {
            get { return _playersFound; }
            set
            {
                _playersFound = value;
                RaisePropertyChanged();
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
                    RaisePropertyChanged();
                }
            }
        }

        public string Title => StaticTitle;

        protected override void DisposeManagedResources()
        {
            base.DisposeManagedResources();
            _cancelatioTokenSource?.Cancel();
        }

        private void Stop()
        {
            IsBusy = true;

            Task.Factory.StartNew(async () =>
            {
                try
                {
                    _cancelatioTokenSource?.Cancel();
                    await SaveAsync();
                }
                finally
                {
                    IsBusy = false;
                    InProcess = false;
                }
            });
        }

        private async Task SaveAsync()
        {
            var found = PlayersFound?.Where(x => _totalPlayers.ContainsKey(x.Item1))
                .Select(x => new { Id = _totalPlayers[x.Item1], steamId = x.Item2 })
                .ToDictionary(x => x.Id, x => x.steamId);

            if ((found != null) && (found.Count > 0))
                await _playerRepository.SaveSteamIdAsync(found);

            InProcess = false;
        }


        private async Task StartAsync()
        {
            InProcess = true;
            var found = PlayersFound;
            found.Clear();

            if (TotalPlayers == null)
                TotalPlayers = (await _playerRepository.GetAllPlayersWithoutSteamAsync())
                    .Where(x => string.IsNullOrEmpty(x.GUID) == false)
                    .GroupBy(x => x.GUID)
                    .ToDictionary(x => x.Key, x => x.First().Id);

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
                    return;
            }

            SaveAsync().Wait();
        }

        private void CheckHash(string currentId, string hash)
        {
            if (TotalPlayers?.ContainsKey(hash) == true)
                PlayersFound.Add(new Tuple<string, string>(hash, currentId));
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
                result += b.ToString("x2");

            return result;
        }
    }
}