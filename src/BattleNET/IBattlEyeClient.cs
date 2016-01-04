using System;

namespace BattleNET
{
    public interface IBattlEyeClient : IDisposable
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