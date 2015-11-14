using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using Arma3BEClient.Common.Logging;
using Arma3BEClient.Contracts;
using Arma3BEClient.Libs.ModelCompact;
using Arma3BEClient.Libs.Repositories;
using Arma3BEClient.Libs.Tools;

namespace Arma3BEClient.ViewModel
{
    public class OptionsModel : DisposableViewModelBase
    {
        private readonly ILog _log = new Log();
        private SettingsStore _settingsStore;

        public OptionsModel()
        {
            using (var servierInfoRepository = new ServerInfoRepository())
            {
                Servers = servierInfoRepository.GetServerInfo().Select(x => new ServerInfoModel(x)).ToList();
            }
        }

        public List<ServerInfoModel> Servers { get; set; }

        public SettingsStore Settings
        {
            get { return _settingsStore ?? (_settingsStore = SettingsStore.Instance); }
            set { _settingsStore = value; }
        }

        public IList<Type> NewListItemTypes
        {
            get { return new List<Type> {typeof (ServerInfoModel)}; }
        }

        public void Save()
        {
            try
            {
                Settings.AdminName = Settings.AdminName.Replace(" ", string.Empty);
                Settings.Save();

                using (var servierInfoRepository = new ServerInfoRepository())
                {
                    var all = servierInfoRepository.GetServerInfo();

                    var todelete = all.Where(x => Servers.All(s => s.GetDbModel().Id != x.Id));

                    foreach (var serverInfo in todelete)
                    {
                        servierInfoRepository.Remove(serverInfo.Id);
                    }

                    foreach (var s in Servers)
                    {
                        var m = s.GetDbModel();
                        if (m.Id == Guid.Empty)
                        {
                            m.Id = Guid.NewGuid();
                        }
                        servierInfoRepository.AddOrUpdate(m);
                    }
                }
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

        public ServerInfo GetDbModel()
        {
            return _info;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}