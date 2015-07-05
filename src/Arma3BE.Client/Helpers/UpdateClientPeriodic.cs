using System;
using System.Threading;
using Arma3BEClient.Common.Core;
using Arma3BEClient.Common.Logging;
using Arma3BEClient.Updater;

namespace Arma3BEClient.Helpers
{
    public class UpdateClientPeriodic : DisposeObject
    {
        private readonly ILog _log;
        private readonly UpdateClient _updateClient;
        //private readonly Timer _updateTimerPlayers;
        //private readonly Timer _updateTimerBans;
        //private readonly Timer _updateTimerAdmins;
        private readonly Timer _updateTimerKeepAlive;

        public UpdateClientPeriodic(UpdateClient updateClient, ILog log)
        {
            _updateClient = updateClient;
            _log = log;


            //_updateTimerPlayers = new Timer((args) =>
            //{
            //    try
            //    {
            //        if (_updateClient.Connected)
            //        {
            //            _updateClient.SendCommand(UpdateClient.CommandType.Players);

            //        }
            //    }
            //    catch (Exception e)
            //    {
            //        _log.Error(e);
            //    }
            //});

            //_updateTimerBans = new Timer((args) =>
            //{
            //    try
            //    {
            //        if (_updateClient.Connected)
            //        {
            //            _updateClient.SendCommand(UpdateClient.CommandType.Bans);

            //        }
            //    }
            //    catch (Exception e)
            //    {
            //        _log.Error(e);
            //    }
            //});

            //_updateTimerAdmins = new Timer((args) =>
            //{
            //    try
            //    {
            //        if (_updateClient.Connected)
            //        {
            //            _updateClient.SendCommand(UpdateClient.CommandType.Admins);

            //        }
            //    }
            //    catch (Exception e)
            //    {
            //        _log.Error(e);
            //    }
            //});


            _updateTimerKeepAlive = new Timer(args =>
            {
                try
                {
                    if (!_updateClient.Connected && !_updateClient.Disposed)
                    {
                        _updateClient.Disconnect();
                        _updateClient.Connect();
                    }
                }
                catch (Exception e)
                {
                    _log.Error(e);
                }
            });
        }

        public void Start()
        {
            //_updateTimerPlayers.Change(0, 15000);
            //_updateTimerBans.Change(500, 120000);
            //_updateTimerAdmins.Change(800, 120000);
            //_updateTimerKeepAlive.Change(300654, 300000);

            _updateTimerKeepAlive.Change(0, 300000);
        }

        protected override void DisposeManagedResources()
        {
            base.DisposeManagedResources();

            //_updateTimerPlayers.Dispose();
            //_updateTimerBans.Dispose();
            //_updateTimerAdmins.Dispose();


            _updateTimerKeepAlive.Dispose();
        }
    }
}