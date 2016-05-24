using Arma3BE.Client.Infrastructure;
using Arma3BE.Client.Modules.MainModule.Properties;
using Arma3BE.Server.Models;
using Arma3BEClient.Common.Attributes;
using Microsoft.Practices.Unity;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Arma3BE.Client.Modules.MainModule.Helpers.Views
{
    public class PlayerView : INotifyPropertyChanged
    {
        public PlayerView()
        {
            _ipService = MainModuleInit.Current.Resolve<IIpService>();
        }

        private IIpService _ipService;

        private string _country;

        [ShowInUi]
        [EnableCopy]
        public int Num { get; set; }

        [ShowInUi]
        [EnableCopy]
        public string Name { get; set; }

        [ShowInUi]
        [EnableCopy]
        public string Comment { get; set; }

        [ShowInUi]
        public bool CanBeAdmin { get; set; }

        [ShowInUi]
        [EnableCopy]
        public string Country
        {
            get
            {
                if (string.IsNullOrEmpty(_country))
                {
                    _country = _ipService.GetCountryLocal(IP);
                }

                return _country;
            }
        }

        [ShowInUi]
        [EnableCopy]
        public Player.PlayerState State { get; set; }

        [ShowInUi]
        [EnableCopy]
        public int Ping { get; set; }

        [ShowInUi]
        [EnableCopy]
        public string IP { get; set; }

        [ShowInUi]
        [EnableCopy]
        public string Guid { get; set; }

        [ShowInUi]
        [EnableCopy]
        public int Port { get; set; }

        public Guid Id { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}