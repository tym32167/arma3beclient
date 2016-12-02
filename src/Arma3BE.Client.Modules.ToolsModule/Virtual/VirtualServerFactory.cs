using Arma3BE.Server.Abstract;
using BattleNET;
using System;

namespace Arma3BE.Client.Modules.ToolsModule.Virtual
{
    public class VirtualServerFactory : IBattlEyeServerFactory
    {
        private Lazy<VirtualServer> _server = new Lazy<VirtualServer>(() => new VirtualServer());

        public IBattlEyeServer Create(BattlEyeLoginCredentials credentials)
        {
            return _server.Value;
        }
    }

    public class VirtualServer : IBattlEyeServer
    {
        public void Dispose()
        {

        }



        public bool Connected { get; private set; }


        public int SendCommand(BattlEyeCommand command, string parameters = "")
        {
            OnCommandRecieved(new CommandArgs(command, parameters));
            return 1;
        }

        public int SendCommand(string command)
        {
            OnPureCommandRecieved(new PureCommandArgs(command));

            return 1;
        }

        public void Disconnect()
        {
            Connected = false;
        }

        public event BattlEyeMessageEventHandler BattlEyeMessageReceived;
        public event BattlEyeConnectEventHandler BattlEyeConnected;
        public event BattlEyeDisconnectEventHandler BattlEyeDisconnected;

        public event EventHandler<CommandArgs> CommandRecieved;
        public event EventHandler<PureCommandArgs> PureCommandRecieved;


        public BattlEyeConnectionResult Connect()
        {
            Connected = true;
            return BattlEyeConnectionResult.Success;
        }

        public void OnBattlEyeConnected(BattlEyeConnectEventArgs args)
        {
            BattlEyeConnected?.Invoke(args);
        }

        public void OnBattlEyeDisconnected(BattlEyeDisconnectEventArgs args)
        {
            BattlEyeDisconnected?.Invoke(args);
        }

        public void OnBattlEyeMessageReceived(BattlEyeMessageEventArgs args)
        {
            BattlEyeMessageReceived?.Invoke(args);
        }


        public class CommandArgs : EventArgs
        {
            public BattlEyeCommand Command { get; }
            public string Parameters { get; }

            public CommandArgs(BattlEyeCommand command, string parameters = "")
            {
                Command = command;
                Parameters = parameters;
            }

        }

        public class PureCommandArgs : EventArgs
        {
            public string Command { get; }

            public PureCommandArgs(string command)
            {
                Command = command;
            }

        }

        protected virtual void OnCommandRecieved(CommandArgs e)
        {
            CommandRecieved?.Invoke(this, e);

        }

        protected virtual void OnPureCommandRecieved(PureCommandArgs e)
        {
            PureCommandRecieved?.Invoke(this, e);
        }
    }
}