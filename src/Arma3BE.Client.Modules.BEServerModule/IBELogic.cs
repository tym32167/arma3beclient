using Arma3BE.Client.Infrastructure.Events.BE;
using System;

namespace Arma3BE.Client.Modules.BEServerModule
{
    public interface IBELogic
    {
        event EventHandler<ServerCommandEventArgs> ServerUpdateHandler;
    }

    public class ServerCommandEventArgs : EventArgs
    {
        public BECommand Command { get; }

        public ServerCommandEventArgs(BECommand command)
        {
            Command = command;
        }
    }

}