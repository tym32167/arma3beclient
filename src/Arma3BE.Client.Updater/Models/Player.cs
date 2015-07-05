using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Arma3BEClient.Updater.Models
{
    public class Player
    {
        protected bool Equals(Player other)
        {
            return _num == other._num 
                && string.Equals(_ip, other._ip) 
                && _port == other._port 
                && string.Equals(_guid, other._guid) 
                && string.Equals(_name, other._name) 
                && _state == other._state;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = _num;
                hashCode = (hashCode*397) ^ (_ip != null ? _ip.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ _port;
                hashCode = (hashCode*397) ^ _ping;
                hashCode = (hashCode*397) ^ (_guid != null ? _guid.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (_name != null ? _name.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (int) _state;
                return hashCode;
            }
        }

        private readonly int _num;
        private readonly string _ip;
        private readonly int _port;
        private readonly int _ping;
        private readonly string _guid;
        private readonly string _name;
        private readonly PlayerState _state;

        public Player(int num, string ip, int port, int ping, string guid, string name, Player.PlayerState state)
        {
            _num = num;
            _ip = ip;
            _port = port;
            _ping = ping;
            _guid = guid;
            _name = name;
            _state = state;
        }


        public static Player Parse(string input)
        {
            try
            {

                var regex = new Regex(@"(\d{1,3})[ ]+(\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}):([\d]+)[ ]+(-?[\d]+)[ ]+([^ ]+)[ ]+(.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                var match = regex.Match(input);

                if (match.Success || match.Groups.Count != 6)
                {
                    var num = Int32.Parse(match.Groups[1].Value);
                    var ip = match.Groups[2].Value;
                    var port = Int32.Parse(match.Groups[3].Value);
                    var ping = Int32.Parse(match.Groups[4].Value);
                    var guid = match.Groups[5].Value;

                    var ind = guid.IndexOf("(", StringComparison.Ordinal);
                    guid = guid.Substring(0, ind);

                    var name = match.Groups[6].Value;

                    var state = Player.PlayerState.Ingame;

                    if (name.EndsWith(" (Lobby)"))
                    {
                        state = PlayerState.Lobby;
                        name = name.Substring(0, name.Length - " (Lobby)".Length);
                    }

                    return new Player(num, ip, port, ping, guid, name, state);
                }

                return null;
            }
            catch(Exception e)
            {
                return null;
            }
        }


        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Player) obj);
        }

        public int Num
        {
            get { return _num; }
        }

        public string IP
        {
            get { return _ip; }
        }

        public int Port
        {
            get { return _port; }
        }

        public int Ping
        {
            get { return _ping; }
        }

        public string Guid
        {
            get { return _guid; }
        }

        public string Name
        {
            get { return _name; }
        }

        public PlayerState State
        {
            get { return _state; }
        }

        public enum PlayerState { Lobby, Ingame }
    }
}