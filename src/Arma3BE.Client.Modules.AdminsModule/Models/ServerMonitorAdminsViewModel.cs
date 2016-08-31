using Arma3BE.Client.Infrastructure.Commands;
using Arma3BE.Client.Infrastructure.Events.BE;
using Arma3BE.Client.Infrastructure.Models;
using Arma3BE.Client.Modules.AdminsModule.Helpers;
using Arma3BE.Server;
using Arma3BEClient.Common.Logging;
using Arma3BEClient.Libs.ModelCompact;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using Admin = Arma3BE.Server.Models.Admin;

namespace Arma3BE.Client.Modules.AdminsModule.Models
{
    public class ServerMonitorAdminsViewModel : ServerMonitorBaseViewModel<Admin, Admin>
    {
        private readonly AdminHelper _helper;
        private readonly ILog _log;

        public ServerMonitorAdminsViewModel(ILog log, ServerInfo serverInfo, IEventAggregator eventAggregator)
            : base(new ActionCommand(() => SendCommand(eventAggregator, serverInfo.Id, CommandType.Admins)), new AdminComparer())
        {
            _log = log;
            _helper = new AdminHelper(_log, serverInfo.Id);

            eventAggregator.GetEvent<BEMessageEvent<BEItemsMessage<Admin>>>().Subscribe(e =>
            {
                if (e.ServerId == serverInfo.Id)
                {
                    SetData(e.Items);
                    WaitingForEvent = false;
                }
            });
        }

        public string Title { get { return "Admins"; } }

        protected override IEnumerable<Admin> RegisterData(IEnumerable<Admin> initialData)
        {
            var enumerable = initialData as IList<Admin> ?? initialData.ToList();
            _helper.RegisterAdmins(enumerable);
            return enumerable;
        }

        private static void SendCommand(IEventAggregator eventAggregator, Guid serverId, CommandType commandType,
            string parameters = null)
        {
            eventAggregator.GetEvent<BEMessageEvent<BECommand>>()
                .Publish(new BECommand(serverId, commandType, parameters));
        }

        private class AdminComparer : IEqualityComparer<Admin>
        {
            public bool Equals(Admin x, Admin y)
            {
                return x.Num == y.Num && x.IP == y.IP && x.Port == y.Port;
            }

            public int GetHashCode(Admin obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}