using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using Arma3BEClient.Boxes;
using Arma3BEClient.Commands;
using Arma3BEClient.Common.Logging;
using Arma3BEClient.Helpers;
using Arma3BEClient.Helpers.Views;
using Arma3BEClient.Libs.Context;
using Arma3BEClient.Updater;
using Ban = Arma3BEClient.Updater.Models.Ban;

namespace Arma3BEClient.Models
{
    public class ServerMonitorBansViewModel : ServerMonitorBaseViewModel<Ban, Helpers.Views.BanView>
    {
        private readonly ILog _log;
        private readonly Guid _serverInfoId;
        private readonly UpdateClient _updateClient;
        private readonly BanHelper _helper;
        private readonly PlayerHelper _playerHelper;

        public ServerMonitorBansViewModel(ILog log, Guid serverInfoId, UpdateClient updateClient)
            : base(new ActionCommand(() => updateClient.SendCommandAsync(UpdateClient.CommandType.Bans)))
        {
            _log = log;
            _serverInfoId = serverInfoId;
            _updateClient = updateClient;
            _helper = new BanHelper(_log, serverInfoId);

            _playerHelper = new PlayerHelper(_log, serverInfoId, _updateClient);


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

                        _updateClient.SendCommandAsync(UpdateClient.CommandType.Bans);
                    }) { IsBackground = true };

                    t.Start();
                }
            });

            CustomBan = new ActionCommand(() =>
            {
                var w = new BanPlayerWindow(_playerHelper, null, false, null, null);
                w.ShowDialog();
            });
        }

        protected override IEnumerable<Helpers.Views.BanView> RegisterData(IEnumerable<Ban> initialData)
        {
            var enumerable = initialData as IList<Ban> ?? initialData.ToList();
            _helper.RegisterBans(enumerable);
            return _helper.GetBanView(enumerable);
        }

        public async void RemoveBan(BanView si)
        {
            await _updateClient.SendCommandAsync(UpdateClient.CommandType.RemoveBan, si.Num.ToString());
            await _updateClient.SendCommandAsync(UpdateClient.CommandType.Bans);
        }


        public override void SetData(IEnumerable<Ban> initialData)
        {
            base.SetData(initialData);
            RaisePropertyChanged("AvailibleBans");
            RaisePropertyChanged("AvailibleBansCount");
        }

        public IEnumerable<Helpers.Views.BanView> SelectedAvailibleBans { get; set; }

        public IEnumerable<BanView> AvailibleBans
        {
            get
            {
                if (_data == null) return new List<BanView>();

                using (var dc = new Arma3BeClientContext())
                {
                    var dbBans = dc.Bans.Where(x => x.ServerInfo.Active && x.IsActive && x.MinutesLeft == 0).ToList();

                    var data = _data.ToList();

                    var res =
                        dbBans.Where(x => data.All(y => y.GuidIp != x.GuidIp)).GroupBy(x => x.GuidIp)
                        .Select(x => x.OrderByDescending(y => y.Reason).First())
                        .Select(x => new BanView()
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

        /*
        protected override IEnumerable<Helpers.Views.BanView> FilterData(IEnumerable<Helpers.Views.BanView> initialData)
        {
            if (initialData == null || string.IsNullOrEmpty(Filter)) return initialData;

            return initialData.Where(x =>
                (!string.IsNullOrEmpty(x.) && x.Comment.Contains(Filter))
                || x.Guid == Filter
                || x.IP.Contains(Filter)
                || x.Name.Contains(Filter)
                || x.Num.ToString() == Filter
                || x.Ping.ToString() == Filter
                || x.Port.ToString() == Filter
                || x.State.ToString() == Filter).ToList();
        }*/
    }
}