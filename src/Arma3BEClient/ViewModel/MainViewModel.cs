using System;
using System.Collections.Generic;
using System.Linq;
using Arma3BEClient.Lib.Context;
using Arma3BEClient.Lib.ModelCompact;
using GalaSoft.MvvmLight;

namespace Arma3BEClient.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            InitServers();
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

        public List<ServerInfo> Servers
        {
            get
            {
                using(var context = new Arma3BeClientContext())
                    return context.ServerInfo.Where(x=>!x.Active).OrderBy(x=>x.Name).ToList();
            }
        }

        public void SetActive(Guid serverId, bool active = false)
        {
            using (var context = new Arma3BeClientContext())
            {
                var server = context.ServerInfo.FirstOrDefault(x => x.Id == serverId);
                if (server != null)
                {
                    server.Active = active;
                    context.SaveChanges();
                }
            }
        }
    }
}