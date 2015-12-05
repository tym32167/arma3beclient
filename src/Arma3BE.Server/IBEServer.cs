using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Arma3BE.Server.Models;

namespace Arma3BE.Server
{
    public interface IBEServer : IDisposable
    {
        bool Connected { get; }
        //bool Disposed { get; }
        event EventHandler<UpdateClientEventArgs<IEnumerable<Player>>> PlayerHandler;
        event EventHandler<UpdateClientEventArgs<IEnumerable<Ban>>> BanHandler;
        event EventHandler<UpdateClientEventArgs<IEnumerable<Admin>>> AdminHandler;
        event EventHandler<UpdateClientEventArgs<IEnumerable<Mission>>> MissionHandler;
        event EventHandler<ChatMessage> ChatMessageHandler;
        event EventHandler<LogMessage> RConAdminLog;
        event EventHandler<LogMessage> PlayerLog;
        event EventHandler<LogMessage> BanLog;
        event EventHandler ConnectHandler;
        event EventHandler ConnectingHandler;
        event EventHandler DisconnectHandler;

        Task SendCommandAsync(CommandType type, string parameters = null);
        //void SendCommand(CommandType type, string parameters = null);

        void Connect();
        void Disconnect();
    }
}