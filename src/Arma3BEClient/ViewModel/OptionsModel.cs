using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using Arma3BEClient.Common.Logging;
using Arma3BEClient.Lib.Context;
using Arma3BEClient.Lib.ModelCompact;
using Arma3BEClient.Lib.Tools;
using GalaSoft.MvvmLight;

namespace Arma3BEClient.ViewModel
{
    public class OptionsModel : ViewModelBase
    {
        private readonly Arma3BeClientContext _context;
        private ILog _log = new Log();
        
        public OptionsModel()
        {
            _context = new Arma3BeClientContext();
            _context.ServerInfo.Load();

            Servers = _context.ServerInfo.Local.Select(x => new ServerInfoModel(x)).ToList();
        }

        public override void Cleanup()
        {
            _context.Dispose();
            base.Cleanup();
        }
        

        public List<ServerInfoModel> Servers { get; set; }



        private SettingsStore _settingsStore;
        public SettingsStore Settings
        {
            get { return _settingsStore ?? (_settingsStore = SettingsStore.Instance); }
            set
            {
                _settingsStore = value;
            }
        }


        public IList<Type> NewListItemTypes
        {
            get { return new List<Type>() {typeof (ServerInfoModel)}; }
        }

        public ObservableCollection<ServerInfo> Local
        {
            get { return _context.ServerInfo.Local; }
        }

        public void Save()
        {

            try
            {
                Settings.AdminName = Settings.AdminName.Replace(" ", string.Empty);
                Settings.Save();
                

                foreach (var s in Servers)
                {
                    var m = s.GetDbModel();
                    if (m.Id == Guid.Empty)
                    {
                        m.Id = Guid.NewGuid();
                        _context.ServerInfo.Add(m);
                    }
                }


                _context.SaveChanges();
            }
            catch (Exception e)
            {
                _log.Error(e);
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    public class ServerInfoModel 
    {
        private readonly ServerInfo _info;

        public ServerInfoModel(ServerInfo info)
        {
            _info = info;

            if (_info != null)
            {
                Host = _info.Host;
                Port = _info.Port;
                Password = _info.Password;
                Name = _info.Name;
            }
        }

        public ServerInfoModel()
        {
            var model = new ServerInfo();
            model.Id = Guid.Empty;
            _info = model;
        }


        public ServerInfo GetDbModel()
        {
            return _info;
        }

        [Required]
        public string Host
        {
            get { return _info.Host; }
            set { _info.Host = value; }
        }

        [Required]
        public int Port
        {
            get { return _info.Port; }
            set { _info.Port = value; }
        }

        [Required]
        public string Password
        {
            get { return _info.Password; }
            set { _info.Password = value; }
        }

        [Required]
        public string Name
        {
            get { return _info.Name; }
            set { _info.Name = value; }
        }
       
        public override string ToString()
        {
            return Name;
        }
    }
}