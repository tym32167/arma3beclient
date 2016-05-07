using Arma3BE.Server.Abstract;
using Arma3BE.Server.Messaging;
using Arma3BE.Server.Models;

namespace Arma3BE.Server.Recognizers
{
    public class ChatMessageRecognizer : IServerMessageRecognizer
    {
        public ServerMessageType GetMessageType(ServerMessage message)
        {
            return ServerMessageType.ChatMessage;
        }

        public bool CanRecognize(ServerMessage serverMessage)
        {
            if (serverMessage.Message.StartsWith("(Side)") || serverMessage.Message.StartsWith("(Vehicle)") ||
                (serverMessage.Message.StartsWith("(Global)") || serverMessage.Message.StartsWith("(Group)")) ||
                (serverMessage.Message.StartsWith("(Command)") || serverMessage.Message.StartsWith("(Direct)")))
                return true;
            if (serverMessage.Message.StartsWith("RCon") && !serverMessage.Message.EndsWith("logged in"))
                return true;

            return false;
        }
    }
}