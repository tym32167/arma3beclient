namespace BattleNET
{
    public interface IBattlEyeClient
    {
        bool Connected { get; }
        bool ReconnectOnPacketLoss { get; set; }
        int SendCommand(BattlEyeCommand command, string parameters = "");
        void Disconnect();
        event BattlEyeMessageEventHandler BattlEyeMessageReceived;
        event BattlEyeConnectEventHandler BattlEyeConnected;
        event BattlEyeDisconnectEventHandler BattlEyeDisconnected;
        BattlEyeConnectionResult Connect();
    }
}