using Arma3BE.Client.Infrastructure.Commands;
using Arma3BE.Client.Infrastructure.Events.BE;
using Arma3BE.Client.Infrastructure.Helpers.Views;
using Arma3BE.Client.Infrastructure.Models;
using Arma3BE.Client.Modules.BanModule.Boxes;
using Arma3BE.Client.Modules.BanModule.Helpers;
using Arma3BE.Server;
using Arma3BE.Server.Models;
using Arma3BEClient.Common.Logging;
using Arma3BEClient.Libs.Repositories;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Input;

namespace Arma3BE.Client.Modules.BanModule.Models
{
    public class ServerMonitorBansViewModel : ServerMonitorBaseViewModel<Ban, BanView>, IServerMonitorBansViewModel
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly BanHelper _helper;
        private readonly ILog _log;
        private readonly Guid _serverInfoId;

        public ServerMonitorBansViewModel(ILog log, Guid serverInfoId, IEventAggregator eventAggregator)
            : base(new ActionCommand(() => SendCommand(eventAggregator, serverInfoId, CommandType.Bans)))
        {
            _log = log;
            _serverInfoId = serverInfoId;
            _eventAggregator = eventAggregator;
            _helper = new BanHelper(_log, eventAggregator);

            SyncBans = new ActionCommand(() =>
            {
                var bans = SelectedAvailibleBans;

                if (bans != null)
                {
                    var t = new Thread(() =>
                    {
                        foreach (var ban in bans)
                        {
                            _helper.BanGUIDOffline(_serverInfoId, ban.GuidIp, ban.Reason, ban.Minutesleft, true);
                            Thread.Sleep(10);
                        }

                        SendCommand(CommandType.Bans);
                    })
                    { IsBackground = true };

                    t.Start();
                }
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
                        SetData(e.Items);
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
            _helper.RegisterBans(enumerable, _serverInfoId);
            return _helper.GetBanView(enumerable);
        }

        public async void RemoveBan(BanView si)
        {
            SendCommand(CommandType.RemoveBan, si.Num.ToString());
            SendCommand(CommandType.Bans);
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

        private static void SendCommand(IEventAggregator eventAggregator, Guid serverId, CommandType commandType, string parameters = null)
        {
            eventAggregator.GetEvent<BEMessageEvent<BECommand>>()
                .Publish(new BECommand(serverId, commandType, parameters));
        }
    }
}