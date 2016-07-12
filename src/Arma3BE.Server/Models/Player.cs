using System;
using System.Text.RegularExpressions;

namespace Arma3BE.Server.Models
{
    public class Player
    {
        public enum PlayerState
        {
            Lobby,
            Ingame
        }

        public Player(int num, string ip, int port, int ping, string guid, string name, PlayerState state)
        {
            Num = num;
            IP = ip;
            Port = port;
            Ping = ping;
            Guid = guid;
            Name = name;
            State = state;
        }

        public int Num { get; }

        public string IP { get; }

        public int Port { get; }

        public int Ping { get; }

        public string Guid { get; }

        public string Name { get; }

        public PlayerState State { get; }

        protected bool Equals(Player other)
        {
            return Num == other.Num
                   && string.Equals(IP, other.IP)
                   && Port == other.Port
                   && string.Equals(Guid, other.Guid)
                   && string.Equals(Name, other.Name)
                   && State == other.State;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Num;
                hashCode = (hashCode * 397) ^ (IP?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ Port;
                hashCode = (hashCode * 397) ^ Ping;
                hashCode = (hashCode * 397) ^ (Guid?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (Name?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (int)State;
                return hashCode;
            }
        }


        private static readonly Regex CompidelRegex = new Regex(
                        @"(\d{1,3})[ ]+(\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}):([\d]+)[ ]+(-?[\d]+)[ ]+([^ ]+)[ ]+(.*)",
                        RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static Player Parse(string input)
        {
            try
            {
                var match = CompidelRegex.Match(input);

                if (match.Success || match.Groups.Count != 6)
                {
                    int num;
                    if (!int.TryParse(match.Groups[1].Value, out num)) return null;
                    var ip = match.Groups[2].Value;
                    int port;
                    if (!int.TryParse(match.Groups[3].Value, out port)) return null;

                    int ping;
                    if (!int.TryParse(match.Groups[4].Value, out ping)) return null;

                    var guid = match.Groups[5].Value;


                    var ind = guid.IndexOf("(", StringComparison.Ordinal);
                    guid = guid.Substring(0, ind);

                    var name = match.Groups[6].Value;

                    var state = PlayerState.Ingame;

                    if (name.EndsWith(" (Lobby)"))
                    {
                        state = PlayerState.Lobby;
                        name = name.Substring(0, name.Length - " (Lobby)".Length);
                    }

                    return new Player(num, ip, port, ping, guid, name, state);
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Player)obj);
        }
    }
}