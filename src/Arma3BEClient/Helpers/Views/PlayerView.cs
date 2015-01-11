using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Arma3BEClient.Annotations;
using Arma3BEClient.Common.Attributes;

namespace Arma3BEClient.Helpers.Views
{
    public class PlayerView : INotifyPropertyChanged
    {
        [ShowInUi]
        [EnableCopy]
        public int Num { get; set; }

        [ShowInUi]
        [EnableCopy]
        public string Name { get; set; }

        [ShowInUi]
        [EnableCopy]
        public string Comment { get; set; }

        private string _country;

        [ShowInUi]
        [EnableCopy]
        public string Country
        {
            get
            {
                if (string.IsNullOrEmpty(_country))
                {
                    IPInfo.Get(IP).ContinueWith(x =>
                    {
                        _country = x.Result.Split(Environment.NewLine.ToCharArray())[2];
                        OnPropertyChanged();
                    });
                }

                return _country;
            }
        }

        [ShowInUi]
        [EnableCopy]
        public Updater.Models.Player.PlayerState State { get; set; }

        [ShowInUi]
        [EnableCopy]
        public string IP { get; set; }

        [ShowInUi]
        [EnableCopy]
        public string Guid { get; set; }

        [ShowInUi]
        [EnableCopy]
        public int Port { get; set; }

        [ShowInUi]
        [EnableCopy]
        public int Ping { get; set; }

        public Guid Id { get; set; }
        

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}