using BattleNET;
using System;

namespace Arma3BE.Server.Abstract
{
    public interface IBattlEyeServer : IDisposable
    {
        bool Connected { get; }
        int SendCommand(BattlEyeCommand command, string parameters = "");
        int SendCommand(string command);
        void Disconnect();
        event BattlEyeMessageEventHandler BattlEyeMessageReceived;
        event BattlEyeConnectEventHandler BattlEyeConnected;
        event BattlEyeDisconnectEventHandler BattlEyeDisconnected;
        BattlEyeConnectionResult Connect();
    }
}