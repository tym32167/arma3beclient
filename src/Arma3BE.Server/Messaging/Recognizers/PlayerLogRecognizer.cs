using Arma3BE.Server.Models;

namespace Arma3BE.Server.Messaging.Recognizers
{
    public class PlayerLogRecognizer : IServerMessageRecognizer
    {
        public ServerMessageType GetMessageType(ServerMessage message)
        {
            return  ServerMessageType.PlayerLog;
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