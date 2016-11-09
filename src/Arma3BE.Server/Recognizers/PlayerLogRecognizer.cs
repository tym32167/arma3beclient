using Arma3BE.Server.Abstract;
using Arma3BE.Server.Messaging;
using Arma3BE.Server.Models;

namespace Arma3BE.Server.Recognizers
{
    public class PlayerLogRecognizer : IServerMessageRecognizer
    {
        public ServerMessageType GetMessageType(ServerMessage message)
        {
            return ServerMessageType.PlayerLog;
        }

        public bool CanRecognize(ServerMessage serverMessage)
        {
            if (serverMessage.Message.StartsWith("Player") && (
                //_message.Contains("connected")
                //||
                serverMessage.Message.EndsWith("disconnected")
                ||
                serverMessage.Message.EndsWith("connected")
                ||
                (serverMessage.Message.Contains("kicked") && !serverMessage.Message.Contains("BattlEye: Admin Ban"))
                ||
                serverMessage.Message.Contains("(unverified)")
                ||
                serverMessage.Message.Contains("is losing connection")
                ))
                return true;

            if (serverMessage.Message.StartsWith("Verified GUID"))
                return true;

            return false;
        }
    }
}