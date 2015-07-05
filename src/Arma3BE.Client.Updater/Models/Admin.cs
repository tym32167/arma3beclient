using System;
using System.Text.RegularExpressions;
using Arma3BEClient.Common.Attributes;

namespace Arma3BEClient.Updater.Models
{
    public class Admin
    {
        private readonly string _ip;
        private readonly int _num;
        private readonly int _port;

        public Admin(int num, string ip, int port)
        {
            _num = num;
            _ip = ip;
            _port = port;
        }

        [ShowInUi]
        [EnableCopy]
        public int Num
        {
            get { return _num; }
        }

        [ShowInUi]
        [EnableCopy]
        public string IP
        {
            get { return _ip; }
        }

        [ShowInUi]
        [EnableCopy]
        public int Port
        {
            get { return _port; }
        }

        protected bool Equals(Admin other)
        {
            return _num == other._num && string.Equals(_ip, other._ip) && _port == other._port;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = _num;
                hashCode = (hashCode*397) ^ (_ip != null ? _ip.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ _port;
                return hashCode;
            }
        }

        public static Admin Parse(string input)
        {
            try
            {
                var regex = new Regex(@"(\d{1,3})[ ]+(\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}):([\d]+)",
                    RegexOptions.Compiled | RegexOptions.IgnoreCase);
                var match = regex.Match(input);

                if (match.Success || match.Groups.Count != 6)
                {
                    var num = int.Parse(match.Groups[1].Value);
                    var ip = match.Groups[2].Value;
                    var port = int.Parse(match.Groups[3].Value);
                    return new Admin(num, ip, port);
                }

                return null;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Admin) obj);
        }
    }
}