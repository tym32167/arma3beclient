using Arma3BE.Server.Abstract;
using BattleNET;

namespace Arma3BE.Server.Mocks
{
    public class MockBattleEyeServer : IBattlEyeServer
    {
        public void Dispose()
        {
        }

        public bool Connected { get; }
        public int SendCommand(BattlEyeCommand command, string parameters = "")
        {
            return 0;
        }

        public void Disconnect()
        {

        }

        public event BattlEyeMessageEventHandler BattlEyeMessageReceived;
        public event BattlEyeConnectEventHandler BattlEyeConnected;
        public event BattlEyeDisconnectEventHandler BattlEyeDisconnected;
        public BattlEyeConnectionResult Connect()
        {
            return BattlEyeConnectionResult.Success;
        }
    }
}