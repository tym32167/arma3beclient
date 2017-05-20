using Arma3BE.Client.Infrastructure;
using Arma3BE.Client.Infrastructure.Commands;
using Arma3BE.Client.Infrastructure.Events;
using Arma3BE.Client.Infrastructure.Events.Models;
using Arma3BE.Client.Infrastructure.Models;
using Arma3BEClient.Libs.ModelCompact;
using Arma3BEClient.Libs.Repositories;
using Prism.Commands;
using Prism.Events;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ExplicitCallerInfoArgument
// ReSharper disable UnusedAutoPropertyAccessor.Global


namespace Arma3BE.Client.Modules.PlayersModule.Models
{
    public class PlayerViewModel : ViewModelBase
    {
        private readonly string _userGuid;
        private readonly IIpService _ipService;
        private readonly IPlayerRepository _playerRepository;
        private readonly IEventAggregator _eventAggregator;

        public PlayerViewModel(string userGuid, IIpService ipService, IPlayerRepository playerRepository, IEventAggregator eventAggregator)
        {
            _userGuid = userGuid;
            _ipService = ipService;
            _playerRepository = playerRepository;
            _eventAggregator = eventAggregator;

            SaveComment = new ActionCommand(SaveUserComment);
            GoToSteamCommand = new ActionCommand(GoToSteam);
            BanCommand = new DelegateCommand<string>(BanPlayerView);

            SetupPlayer();
            SetupPlayerIPInfo();
        }

        private async void SetupPlayer()
        {
            Player = await _playerRepository.GetPlayerInfo(_userGuid);
            OnPropertyChanged(nameof(Player));
        }

        private async void SetupPlayerIPInfo()
        {
            PlayerIPInfo = await _ipService.Get(Player.LastIp);
            OnPropertyChanged(nameof(PlayerIPInfo));
        }


        private void BanPlayerView(string obj)
        {
            _eventAggregator.GetEvent<BanUserEvent>()
                .Publish(new BanUserModel(null, obj, false, null, null));
        }

        private void GoToSteam()
        {
            var id = Player?.SteamId;
            if (string.IsNullOrEmpty(id) == false)
            {
                var suri = $"http://steamcommunity.com/profiles/{id}/";
                Process.Start(new ProcessStartInfo(new Uri(suri).AbsoluteUri));
            }
        }

        public Player Player { get; set; }



        [SuppressMessage("ReSharper", "UnusedMember.Global")]
        public string PlayerIPInfo { get; private set; }

        public ICommand SaveComment { get; set; }
        public ICommand GoToSteamCommand { get; set; }

        public ICommand BanCommand { get; set; }

        private void SaveUserComment()
        {
            _playerRepository.UpdatePlayerComment(Player.GUID, Player.Comment);
            SetupPlayer();
        }
    }
}