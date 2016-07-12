using Arma3BE.Client.Infrastructure.Helpers;
using Arma3BEClient.Common.Logging;
using Arma3BEClient.Libs.Repositories;
using System;
using System.Collections.Generic;
using Admin = Arma3BE.Server.Models.Admin;

namespace Arma3BE.Client.Modules.AdminsModule.Helpers
{
    public class AdminHelper : StateHelper<Admin>
    {
        private readonly Guid _serverInfoId;

        public AdminHelper(ILog log, Guid serverInfoId)
        {
            _serverInfoId = serverInfoId;
        }

        public void RegisterAdmins(IEnumerable<Admin> list)
        {
            using (var repo = new AdminRepository())
            {
                repo.AddOrUpdate(list, _serverInfoId);
            }
        }
    }
}