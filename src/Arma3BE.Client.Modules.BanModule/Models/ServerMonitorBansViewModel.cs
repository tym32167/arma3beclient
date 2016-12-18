using Arma3BE.Client.Infrastructure.Commands;
using Arma3BE.Client.Infrastructure.Events;
using Arma3BE.Client.Infrastructure.Events.BE;
using Arma3BE.Client.Infrastructure.Events.Models;
using Arma3BE.Client.Infrastructure.Helpers;
using Arma3BE.Client.Infrastructure.Helpers.Views;
using Arma3BE.Client.Infrastructure.Models;
using Arma3BE.Client.Modules.BanModule.Boxes;
using Arma3BE.Server;
using Arma3BEClient.Libs.ModelCompact;
using Arma3BEClient.Libs.Repositories;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Ban = Arma3BE.Server.Models.Ban;

namespace Arma3BE.Client.Modules.BanModule.Models
{
    public class ServerMonitorBansViewModel : ServerMonitorBaseViewModel<Ban, BanView>
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IBanHelper _helper;
        private readonly Guid _serverInfoId;

        public ServerMonitorBansViewModel(ServerInfo serverInfo, IEventAggregator eventAggregator,
            IBanHelper banHelper)
            : base(
                new ActionCommand(() => SendCommand(eventAggregator, serverInfo.Id, CommandType.Bans)),
                new BanViewComparer())
        {
            _serverInfoId = serverInfo.Id;
            _eventAggregator = eventAggregator;
            _helper = banHelper;

            SyncBans = new ActionCommand(() =>
            {
                var bans = SelectedAvailibleBans;

                if (bans != null)
                    Task.Factory.StartNew(() => { _helper.BanGUIDOffline(_serverInfoId, bans.ToArray(), true); },
                        TaskCreationOptions.LongRunning);
            });

            CustomBan = new ActionCommand(() =>
            {
                var w = new BanPlayerWindow(_serverInfoId, _helper, null, false, null, null);
                w.ShowDialog();
            });

            _eventAggregator.GetEvent<BEMessageEvent<BEItemsMessage<Ban>>>()
                .Subscribe(e =>
                {
                    if (_serverInfoId == e.ServerId)
                    {
                        SetData(e.Items);
                        WaitingForEvent = false;
                    }
                });
        }

        public string Title
        {
            get { return "Bans"; }
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
            _helper.RegisterBans(enumerable, _serverInfoId);
            return _helper.GetBanView(enumerable);
        }

        public void RemoveBan(BanView si)
        {
            SendCommand(CommandType.RemoveBan, si.Num.ToString());
        }

        public override void SetData(IEnumerable<Ban> initialData)
        {
            base.SetData(initialData);
            OnPropertyChanged(nameof(AvailibleBans));
            OnPropertyChanged(nameof(AvailibleBansCount));
        }

        private void SendCommand(CommandType commandType, string parameters = null)
        {
            SendCommand(_eventAggregator, _serverInfoId, commandType, parameters);
        }

        private static void SendCommand(IEventAggregator eventAggregator, Guid serverId, CommandType commandType,
            string parameters = null)
        {
            eventAggregator.GetEvent<BEMessageEvent<BECommand>>()
                .Publish(new BECommand(serverId, commandType, parameters));
        }

        public void ShowPlayerInfo(BanView si)
        {
            if (string.IsNullOrEmpty(si.GuidIp) == false)
                _eventAggregator.GetEvent<ShowUserInfoEvent>().Publish(new ShowUserModel(si.GuidIp));
        }

        private class BanViewComparer : IEqualityComparer<BanView>
        {
            public bool Equals(BanView x, BanView y)
            {
                return x.GuidIp == y.GuidIp && x.Num == y.Num && x.Reason == y.Reason;
            }

            public int GetHashCode(BanView obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}