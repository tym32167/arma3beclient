using System;
using System.Collections.Generic;
using System.Linq;
using Arma3BEClient.Libs.Context;
using Arma3BEClient.Libs.ModelCompact;
using GalaSoft.MvvmLight;

namespace Arma3BEClient.ViewModel
{
    /// <summary>
    ///     This class contains properties that the main View can data bind to.
    ///     <para>
    ///         Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    ///     </para>
    ///     <para>
    ///         You can also use Blend to data bind with the tool's support.
    ///     </para>
    ///     <para>
    ///         See http://www.galasoft.ch/mvvm
    ///     </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            InitServers();
        }

        public List<ServerInfo> Servers
        {
            get
            {
                using (var repo = new ServerInfoRepository())
                    return repo.GetNotActiveServerInfo().OrderBy(x => x.Name).ToList();
            }
        }

        private void InitServers()
        {
            //using (var context = new Arma3BeClientContext())
            //{
            //    context.ServerInfo.Where(x=>x.Active).ToList().ForEach(x => x.Active = false);
            //    context.SaveChanges();
            //}

            Reload();
        }

        public void Reload()
        {
            RaisePropertyChanged("Servers");
        }

        public void SetActive(Guid serverId, bool active = false)
        {
            using (var repo = new ServerInfoRepository())
            {
                repo.SetServerInfoActive(serverId, active);
            }
        }
    }
}