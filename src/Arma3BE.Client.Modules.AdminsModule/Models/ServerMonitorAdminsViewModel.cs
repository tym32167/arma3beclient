using Arma3BE.Client.Infrastructure.Commands;
using Arma3BE.Client.Infrastructure.Models;
using Arma3BE.Client.Modules.AdminsModule.Helpers;
using Arma3BE.Server;
using Arma3BE.Server.Abstract;
using Arma3BEClient.Common.Logging;
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
        private readonly IBEServer _beServer;

        public ServerMonitorAdminsViewModel(ILog log, Guid serverInfoId, IBEServer beServer)
            : base(new ActionCommand(() => beServer.SendCommand(CommandType.Admins)))
        {
            _log = log;
            _beServer = beServer;
            _helper = new AdminHelper(_log, serverInfoId);
            _beServer.AdminHandler += (s, e) =>
            {
                SetData(e.Data);
            };
        }

        protected override IEnumerable<Admin> RegisterData(IEnumerable<Admin> initialData)
        {
            var enumerable = initialData as IList<Admin> ?? initialData.ToList();
            _helper.RegisterAdmins(enumerable);
            return enumerable;
        }
    }
}