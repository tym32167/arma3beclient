using System;
using System.Diagnostics;
using System.Windows.Input;
using Arma3BE.Client.Infrastructure;
using Arma3BE.Client.Infrastructure.Commands;
using Arma3BE.Client.Infrastructure.Extensions;
using Arma3BE.Client.Infrastructure.Models;
using Arma3BEClient.Libs.ModelCompact;
using Arma3BEClient.Libs.Repositories;
using Prism.Commands;

namespace Arma3BE.Client.Modules.PlayersModule.Models
{
    public class PlayerViewModel : ViewModelBase
    {
        private readonly string _userGuid;
        private readonly IIpService _ipService;
        private Player _player;
        private string _playerIPInfo;

        public PlayerViewModel(string userGuid, IIpService ipService)
        {
            _userGuid = userGuid;
            _ipService = ipService;

            SaveComment = new ActionCommand(SaveUserComment);
            GoToSteamCommand = new ActionCommand(GoToSteam);
        }

        private void GoToSteam()
        {
            var id = _player?.SteamId;
            if (string.IsNullOrEmpty(id) == false)
            {
                var suri = $"http://steamcommunity.com/profiles/{id}/";
                Process.Start(new ProcessStartInfo(new Uri(suri).AbsoluteUri));
            }
        }

        public Player Player
        {
            get
            {
                if (_player == null)
                {
                    using (var dc = PlayerRepositoryFactory.Create())
                    {
                        var player = dc.GetPlayerInfo(_userGuid);
                        _player = player;
                    }
                }
                return _player;
            }
        }

        public string PlayerIPInfo
        {
            get
            {
                if (string.IsNullOrEmpty(_playerIPInfo))
                {
                    var t = _ipService.Get(Player.LastIp);
                    t.ContinueWith(x =>
                    {
                        _playerIPInfo = x.Result;
                        OnPropertyChanged(nameof(PlayerIPInfo));
                    });
                }
                return _playerIPInfo;
            }
        }

        public ICommand SaveComment { get; set; }
        public ICommand GoToSteamCommand { get; set; }

        private void SaveUserComment()
        {
            using (var repo = PlayerRepositoryFactory.Create())
            {
                repo.UpdatePlayerComment(Player.GUID, Player.Comment);
            }

            _player = null;
            OnPropertyChanged(nameof(Player));
        }
    }
}