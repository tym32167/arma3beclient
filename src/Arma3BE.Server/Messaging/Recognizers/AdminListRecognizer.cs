using Arma3BE.Server.Models;

namespace Arma3BE.Server.Messaging.Recognizers
{
    public class AdminListRecognizer : IServerMessageRecognizer
    {
        public ServerMessageType GetMessageType(ServerMessage message)
        {
            return ServerMessageType.AdminList;
        }

        public bool CanRecognize(ServerMessage serverMessage)
        {
            return serverMessage.Message.StartsWith("Connected RCon admins:");
        }
    }
}