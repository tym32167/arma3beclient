using System;
using BattleNET;

namespace Arma3BE.Server.Abstract
{
    public interface IBattlEyeServer : IDisposable
    {
        bool Connected { get; }
        int SendCommand(BattlEyeCommand command, string parameters = "");
        void Disconnect();
        event BattlEyeMessageEventHandler BattlEyeMessageReceived;
        event BattlEyeConnectEventHandler BattlEyeConnected;
        event BattlEyeDisconnectEventHandler BattlEyeDisconnected;
        BattlEyeConnectionResult Connect();
    }
}