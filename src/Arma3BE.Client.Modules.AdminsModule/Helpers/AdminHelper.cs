using Arma3BE.Client.Infrastructure.Helpers;
using Arma3BEClient.Libs.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Admin = Arma3BE.Server.Models.Admin;

namespace Arma3BE.Client.Modules.AdminsModule.Helpers
{
    public class AdminHelper : StateHelper<Admin>
    {
        private readonly Guid _serverInfoId;

        public AdminHelper(Guid serverInfoId)
        {
            _serverInfoId = serverInfoId;
        }

        public Task RegisterAdminsAsync(IEnumerable<Admin> list)
        {
            using (var repo = new AdminRepository())
            {
                return repo.AddOrUpdateAsync(list, _serverInfoId);
            }
        }
    }
}