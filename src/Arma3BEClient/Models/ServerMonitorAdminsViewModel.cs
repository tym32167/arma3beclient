using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Arma3BEClient.Common.Logging;
using Arma3BEClient.Helpers;
using Arma3BEClient.Libs.ModelCompact;
using Admin = Arma3BEClient.Updater.Models.Admin;


namespace Arma3BEClient.Models
{
    public class ServerMonitorAdminsViewModel : ServerMonitorBaseViewModel<Admin, Admin>
    {
        private readonly ILog _log;
        private readonly AdminHelper _helper;

        public ServerMonitorAdminsViewModel(ILog log, ServerInfo serverInfo, ICommand refreshCommand)
            : base(refreshCommand)
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
    }
}