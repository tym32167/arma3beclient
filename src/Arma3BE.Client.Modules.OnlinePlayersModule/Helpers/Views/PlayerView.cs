using Arma3BE.Client.Infrastructure;
using Arma3BE.Client.Infrastructure.Models;
using Arma3BE.Server.Models;
using Arma3BEClient.Common.Attributes;
using Microsoft.Practices.Unity;
using System;
// ReSharper disable UnusedMember.Global
// ReSharper disable ExplicitCallerInfoArgument
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Arma3BE.Client.Modules.OnlinePlayersModule.Helpers.Views
{
    public class PlayerView : ViewModelBase
    {
        private bool _canBeAdmin;
        private string _comment;

        private string _country;
        private string _city;
        private string _ip;

        private readonly IIpService _ipService;
        private int _ping;
        private int _port;
        private Player.PlayerState _state;

        public PlayerView()
        {
            _ipService = OnlinePlayersModuleInit.Current.Resolve<IIpService>();

            _geoInfo = new Lazy<GeoInfo>(() => _ipService.GetGeoInfoLocal(IP));
        }

        [ShowInUi]
        [EnableCopy]
        public int Num { get; set; }

        [ShowInUi]
        [EnableCopy]
        public string Name { get; set; }

        [ShowInUi]
        [EnableCopy]
        public string Comment
        {
            get { return _comment; }
            set { SetProperty(ref _comment, value); }
        }

        [ShowInUi]
        public bool CanBeAdmin
        {
            get { return _canBeAdmin; }
            set { SetProperty(ref _canBeAdmin, value); }
        }

        [ShowInUi]
        [EnableCopy]
        public string Country
        {
            get
            {
                if (string.IsNullOrEmpty(_country))
                {
                    SetProperty(ref _country, _geoInfo?.Value?.Country, nameof(Country));
                }

                return _country;
            }
        }

        private Lazy<GeoInfo> _geoInfo;

        [ShowInUi]
        [EnableCopy]
        public string City
        {
            get
            {
                if (string.IsNullOrEmpty(_city))
                {
                    SetProperty(ref _city, _geoInfo?.Value?.City, nameof(Country));
                }

                return _city;
            }
        }

        [ShowInUi]
        [EnableCopy]
        public Player.PlayerState State
        {
            get { return _state; }
            set { SetProperty(ref _state, value); }
        }

        [ShowInUi]
        [EnableCopy]
        public int Ping
        {
            get { return _ping; }
            set { SetProperty(ref _ping, value); }
        }

        [ShowInUi]
        [EnableCopy]
        public string SteamId { get; set; }

        [ShowInUi]
        [EnableCopy]
        public string IP
        {
            get { return _ip; }
            set { SetProperty(ref _ip, value); }
        }

        [ShowInUi]
        [EnableCopy]
        public string Guid { get; set; }

        [ShowInUi]
        [EnableCopy]
        public int Port
        {
            get { return _port; }
            set { SetProperty(ref _port, value); }
        }

        public Guid Id { get; set; }
    }
}