using Arma3BEClient.Common.Logging;
using Arma3BEClient.Helpers;
using Arma3BEClient.Libs.ModelCompact;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Admin = Arma3BE.Server.Models.Admin;

namespace Arma3BEClient.Models
{
    public class ServerMonitorAdminsViewModel : ServerMonitorBaseViewModel<Admin, Admin>
    {
        private readonly AdminHelper _helper;
        private readonly ILog _log;

        public ServerMonitorAdminsViewModel(ILog log, ServerInfo serverInfo, ICommand refreshCommand)
            : base(refreshCommand, new AdminComparer())
        {
            _log = log;
            _helper = new AdminHelper(_log, serverInfo);
        }

        protected override IEnumerable<Admin> RegisterData(IEnumerable<Admin> initialData)
        {
            var enumerable = initialData as IList<Admin> ?? initialData.ToList();
            _helper.RegisterAdmins(enumerable);
            return enumerable;
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