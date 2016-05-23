using System.Collections.Generic;
using Arma3BEClient.Common.Logging;
using Arma3BEClient.Libs.ModelCompact;
using Arma3BEClient.Libs.Repositories;
using Admin = Arma3BE.Server.Models.Admin;

namespace Arma3BE.Client.Modules.MainModule.Helpers
{
    public class AdminHelper : StateHelper<Admin>
    {
        private readonly ServerInfo _currentServer;

        public AdminHelper(ILog log, ServerInfo currentServer)
        {
            _currentServer = currentServer;
        }

        public void RegisterAdmins(IEnumerable<Admin> list)
        {
            using (var repo = new AdminRepository())
            {
                repo.AddOrUpdate(list, _currentServer.Id);
            }
        }
    }
}