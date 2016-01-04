using Arma3BE.Server.Models;

namespace Arma3BE.Server.Messaging.Recognizers
{
    public class BanLogRecognizer : IServerMessageRecognizer
    {
        public ServerMessageType GetMessageType(ServerMessage message)
        {
            return ServerMessageType.BanLog;
        }

        public bool CanRecognize(ServerMessage serverMessage)
        {
            return serverMessage.Message.StartsWith("Player") && (
                serverMessage.Message.Contains("kicked")
                );
        }
    }
}