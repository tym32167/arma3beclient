using System;
using System.Collections.Generic;
using System.Configuration;
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
            RCon
        }

        private bool? _isImportantMessage = null;
        public bool IsImportantMessage {
            get
            {
                if (_isImportantMessage.HasValue) return _isImportantMessage.Value;
                _isImportantMessage = IsImportantMessageProc(Message);
                return _isImportantMessage.Value;
            } 
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
                if (Message.StartsWith("RCon") &&
                    !Message.Contains("logged in") && !Message.Contains("Connection Lost!")) return MessageType.RCon;

                return MessageType.Unknown;
            }
        }

        private bool IsImportantMessageProc(string message)
        {
            var importantWordsConfig = ConfigurationManager.AppSettings["Important_Words"];
            if (string.IsNullOrEmpty(importantWordsConfig)) return false;
            if (string.IsNullOrEmpty(message)) return false;

            var importantWords = new HashSet<string>(importantWordsConfig.ToLower().Split('|').Distinct());
            var words = new WordParser().Parse(message)
                .Where(x => !string.IsNullOrEmpty(x))
                .Select(x => x.ToLower())
                .Distinct()
                .ToList();
            if (words.Any(x => importantWords.Contains(x))) return true;
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