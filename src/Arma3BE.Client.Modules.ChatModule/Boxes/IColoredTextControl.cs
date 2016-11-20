using System.Collections.Generic;
using Arma3BE.Server.Models;

namespace Arma3BE.Client.Modules.ChatModule.Boxes
{
    public interface IColoredTextControl
    {
        bool IsAutoScroll { get; set; }

        void AppendPlayerText(KeyValuePair<string, string> player, bool isIn);
        void AppendText(ChatMessage message, string servername = null);
        void ClearAll();
    }
}