using System;
using System.Collections.Generic;
using Arma3BE.Server.Models;

namespace Arma3BE.Server.Abstract
{
    public interface IBEServer : IDisposable
    {
        bool Connected { get; }
        //bool Disposed { get; }
        event EventHandler<BEClientEventArgs<IEnumerable<Player>>> PlayerHandler;
        event EventHandler<BEClientEventArgs<IEnumerable<Ban>>> BanHandler;
        event EventHandler<BEClientEventArgs<IEnumerable<Admin>>> AdminHandler;
        event EventHandler<BEClientEventArgs<IEnumerable<Mission>>> MissionHandler;
        event EventHandler<ChatMessage> ChatMessageHandler;
        event EventHandler<LogMessage> RConAdminLog;
        event EventHandler<LogMessage> PlayerLog;
        event EventHandler<LogMessage> BanLog;
        event EventHandler ConnectHandler;
        event EventHandler ConnectingHandler;
        event EventHandler DisconnectHandler;

        //Task SendCommandAsync(CommandType type, string parameters = null);
        void SendCommand(CommandType type, string parameters = null);

        void Connect();
        void Disconnect();
    }
}