using System;
using Arma3BEClient.Common.Attributes;

namespace Arma3BEClient.Helpers.Views
{
    public class PlayerView
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
    }
}