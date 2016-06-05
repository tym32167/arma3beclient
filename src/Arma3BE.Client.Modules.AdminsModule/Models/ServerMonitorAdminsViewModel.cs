using Arma3BE.Client.Infrastructure.Commands;
using Arma3BE.Client.Infrastructure.Events.BE;
using Arma3BE.Client.Infrastructure.Models;
using Arma3BE.Client.Modules.AdminsModule.Helpers;
using Arma3BE.Server;
using Arma3BEClient.Common.Logging;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using Admin = Arma3BE.Server.Models.Admin;

namespace Arma3BE.Client.Modules.AdminsModule.Models
{
    public class ServerMonitorAdminsViewModel : ServerMonitorBaseViewModel<Admin, Admin>, IServerMonitorAdminsViewModel
    {
        private readonly AdminHelper _helper;
        private readonly ILog _log;

        public ServerMonitorAdminsViewModel(ILog log, Guid serverInfoId, IEventAggregator eventAggregator)
            : base(new ActionCommand(() => SendCommand(eventAggregator, serverInfoId, CommandType.Admins)))
        {
            _log = log;
            _helper = new AdminHelper(_log, serverInfoId);

            eventAggregator.GetEvent<BEMessageEvent<BEItemsMessage<Admin>>>().Subscribe(e =>
            {
                if (e.ServerId == serverInfoId)
                    SetData(e.Items);
            });
        }

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
    }
}