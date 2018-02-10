using Arma3BE.Client.Infrastructure.Commands;
using Arma3BE.Client.Infrastructure.Events.BE;
using Arma3BE.Client.Infrastructure.Models;
using Arma3BE.Client.Modules.AdminsModule.Helpers;
using Arma3BE.Server;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Arma3BEClient.Libs.EF.Repositories;
using Admin = Arma3BE.Server.Models.Admin;

namespace Arma3BE.Client.Modules.AdminsModule.Models
{
    public class ServerMonitorAdminsViewModel : ServerMonitorBaseViewModel<Admin, Admin>
    {
        private readonly AdminHelper _helper;

        public ServerMonitorAdminsViewModel(ServerInfoDto serverInfo, IEventAggregator eventAggregator)
            : base(new ActionCommand(() => SendCommand(eventAggregator, serverInfo.Id, CommandType.Admins)), new AdminComparer())
        {
            _helper = new AdminHelper(serverInfo.Id);

            eventAggregator.GetEvent<BEMessageEvent<BEItemsMessage<Admin>>>().Subscribe(async e =>
            {
                if (e.ServerId == serverInfo.Id)
                {
                    await SetDataAsync(e.Items);
                    WaitingForEvent = false;
                }
            });
        }

        public string Title => "Admins";

        protected override async Task<IEnumerable<Admin>> RegisterDataAsync(IEnumerable<Admin> initialData)
        {
            var enumerable = initialData as IList<Admin> ?? initialData.ToList();
            await _helper.RegisterAdminsAsync(enumerable);
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