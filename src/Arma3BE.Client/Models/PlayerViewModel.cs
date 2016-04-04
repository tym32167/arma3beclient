using System.Windows.Input;
using Arma3BEClient.Commands;
using Arma3BEClient.Helpers;
using Arma3BEClient.Libs.ModelCompact;
using Arma3BEClient.Libs.Repositories;
using GalaSoft.MvvmLight;

namespace Arma3BEClient.Models
{
    public class PlayerViewModel : ViewModelBase
    {
        private readonly string _userGuid;
        private Player _player;
        private string _playerIPInfo;

        public PlayerViewModel(string userGuid)
        {
            _userGuid = userGuid;

            SaveComment = new ActionCommand(SaveUserComment);
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
                    var t = IPInfo.Get(Player.LastIp);
                    t.ContinueWith(x =>
                    {
                        _playerIPInfo = x.Result;
                        RaisePropertyChanged("PlayerIPInfo");
                    });
                }
                return _playerIPInfo;
            }
        }

        public ICommand SaveComment { get; set; }

        private void SaveUserComment()
        {
            using (var repo = PlayerRepositoryFactory.Create())
            {
                repo.UpdatePlayerComment(Player.GUID, Player.Comment);
            }

            _player = null;
            RaisePropertyChanged("Player");
        }
    }
}