using System;
using System.Collections.Generic;
using System.Linq;

namespace Arma3BE.Server.Models
{
    public class ChatMessage : MessageBase
    {
        public enum MessageType
        {
            Unknown,
            Side,
            Direct,
            Vehicle,
            Global,
            Group,
            Command,
            RCon,
            NonCommon
        }

        public MessageType Type
        {
            get
            {
                if (Message.StartsWith("(Side)")) return MessageType.Side;
                if (Message.StartsWith("(Vehicle)")) return MessageType.Vehicle;
                if (Message.StartsWith("(Global)")) return MessageType.Global;
                if (Message.StartsWith("(Group)")) return MessageType.Group;
                if (Message.StartsWith("(Command)")) return MessageType.Command;
                if (Message.StartsWith("(Direct)")) return MessageType.Direct;
                if (Message.StartsWith("(Unknown)")) return MessageType.NonCommon;
                if (Message.StartsWith("RCon") &&
                    !Message.Contains("logged in") && !Message.Contains("Connection Lost!")) return MessageType.RCon;

                return MessageType.Unknown;
            }
        }

        public static bool IsImportantMessageProc(ChatMessage message, string[] importantWords)
        {
            if (importantWords == null) return false;
            if (string.IsNullOrEmpty(message.Message)) return false;

            var dict = new HashSet<string>(importantWords.Select(x => x.ToLower()).Distinct());
            var words = new WordParser().Parse(message.Message)
                .Where(x => !string.IsNullOrEmpty(x))
                .Select(x => x.ToLower())
                .Distinct()
                .ToList();
            if (words.Any(x => dict.Contains(x))) return true;
            return false;
        }

        private class WordParser
        {
            public IEnumerable<string> Parse(string input)
            {
                if (string.IsNullOrEmpty(input))
                    throw new ArgumentException(nameof(input));

                var words = new List<string>();

                var list = new List<char>();

                foreach (var c in input)
                {
                    if (char.IsLetterOrDigit(c) || c == '-' || c == '\'')
                    {
                        list.Add(c);
                    }
                    else
                    {
                        var str = new string(list.ToArray());
                        if (CheckWord(str))
                            words.Add(str);
                        list.Clear();
                    }
                }

                if (list.Any())
                {
                    var str = new string(list.ToArray());
                    if (CheckWord(str))
                        words.Add(str);
                }

                return words;
            }

            private bool CheckWord(string word)
            {
                return word.Any(char.IsLetterOrDigit);
            }
        }
    }

    public class LogMessage : MessageBase
    {

    }

    public abstract class MessageBase
    {
        public DateTime Date { get; set; }
        public string Message { get; set; }
    }
}