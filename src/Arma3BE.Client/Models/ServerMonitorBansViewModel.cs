using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using Arma3BE.Server;
using Arma3BE.Server.Models;
using Arma3BEClient.Boxes;
using Arma3BEClient.Commands;
using Arma3BEClient.Common.Logging;
using Arma3BEClient.Helpers;
using Arma3BEClient.Helpers.Views;
using Arma3BEClient.Libs.Repositories;

namespace Arma3BEClient.Models
{
    public class ServerMonitorBansViewModel : ServerMonitorBaseViewModel<Ban, BanView>
    {
        private readonly IBEServer _beServer;
        private readonly BanHelper _helper;
        private readonly ILog _log;
        private readonly PlayerHelper _playerHelper;
        private readonly Guid _serverInfoId;

        public ServerMonitorBansViewModel(ILog log, Guid serverInfoId, IBEServer beServer)
            : base(new ActionCommand(() => beServer.SendCommand(CommandType.Bans)))
        {
            _log = log;
            _serverInfoId = serverInfoId;
            _beServer = beServer;
            _helper = new BanHelper(_log, serverInfoId);

            _playerHelper = new PlayerHelper(_log, serverInfoId, _beServer);


            SyncBans = new ActionCommand(() =>
            {
                //var bans = AvailibleBans.ToList();

                var bans = SelectedAvailibleBans;

                if (bans != null)
                {
                    var t = new Thread(() =>
                    {
                        foreach (var ban in bans)
                        {
                            _playerHelper.BanGUIDOffline(ban.GuidIp, ban.Reason, ban.Minutesleft, true);
                            Thread.Sleep(10);
                        }

                        _beServer.SendCommand(CommandType.Bans);
                    }) {IsBackground = true};

                    t.Start();
                }
            });

            CustomBan = new ActionCommand(() =>
            {
                var w = new BanPlayerWindow(_playerHelper, null, false, null, null);
                w.ShowDialog();
            });
        }

        public IEnumerable<BanView> SelectedAvailibleBans { get; set; }

        public IEnumerable<BanView> AvailibleBans
        {
            get
            {
                if (_data == null) return new List<BanView>();

                using (var dc = new BanRepository())
                {
                    var dbBans = dc.GetActivePermBans();

                    var data = _data.ToList();

                    var res =
                        dbBans.Where(x => data.All(y => y.GuidIp != x.GuidIp)).GroupBy(x => x.GuidIp)
                            .Select(x => x.OrderByDescending(y => y.Reason).First())
                            .Select(x => new BanView
                            {
                                GuidIp = x.GuidIp,
                                Minutesleft = x.MinutesLeft,
                                Num = 0,
                                PlayerComment = x.Player == null ? string.Empty : x.Player.Comment,
                                Reason = x.Reason,
                                PlayerName = x.Player == null ? string.Empty : x.Player.Name
                            })
                            .ToList();

                    return res;
                }
            }
        }

        public long AvailibleBansCount
        {
            get { return AvailibleBans.Count(); }
        }

        public ICommand SyncBans { get; set; }
        public ICommand CustomBan { get; set; }

        protected override IEnumerable<BanView> RegisterData(IEnumerable<Ban> initialData)
        {
            var enumerable = initialData as IList<Ban> ?? initialData.ToList();
            _helper.RegisterBans(enumerable);
            return _helper.GetBanView(enumerable);
        }

        public async void RemoveBan(BanView si)
        {
            _beServer.SendCommand(CommandType.RemoveBan, si.Num.ToString());
            _beServer.SendCommand(CommandType.Bans);
        }

        public override void SetData(IEnumerable<Ban> initialData)
        {
            base.SetData(initialData);
            RaisePropertyChanged("AvailibleBans");
            RaisePropertyChanged("AvailibleBansCount");
        }
    }
}