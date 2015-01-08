using System.Linq;
using System.Windows.Input;
using Arma3BEClient.Commands;
using Arma3BEClient.Helpers;
using Arma3BEClient.Lib.Context;
using Arma3BEClient.Lib.ModelCompact;
using GalaSoft.MvvmLight;

namespace Arma3BEClient.Models
{
    public class PlayerViewModel : ViewModelBase
    {
        private readonly string _userGuid;
      

        public PlayerViewModel(string userGuid)
        {
            _userGuid = userGuid;

            SaveComment = new ActionCommand(SaveUserComment);
        }

        private void SaveUserComment()
        {
            using (var dc = new Arma3BeClientContext())
            {
                var dbp = dc.Player.FirstOrDefault(x => x.GUID == Player.GUID);
                if (dbp != null)
                {
                    dbp.Comment = Player.Comment;
                    dc.SaveChanges();
                }
            }


            _player = null;
            RaisePropertyChanged("Player");
        }


        private Player _player;
        public Player Player
        {
            get
            {
                if (_player == null)
                {
                    using (var dc = new Arma3BeClientContext())
                    {
                        var player = dc.Player.FirstOrDefault(x => x.GUID == _userGuid);
                        if (player != null)
                        {
                            player.Bans = player.Bans.ToList();
                            foreach (var ban in player.Bans)
                            {
                                ban.ServerInfo = ban.ServerInfo;
                            }

                            player.Notes = player.Notes.ToList();
                            player.PlayerHistory = player.PlayerHistory.ToList();
                        }

                        _player = player;
                    }
                }
                return _player;
            }
        }


        private string _playerIPInfo;
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
    }
}