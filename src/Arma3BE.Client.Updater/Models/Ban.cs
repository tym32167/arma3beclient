using System;
using System.Text.RegularExpressions;

namespace Arma3BEClient.Updater.Models
{
    public class Ban
    {
        private readonly string _guidIp;
        private readonly int _minutesleft;
        private readonly int _num;
        private readonly string _reason;

        public Ban(int num, string guidIp, int minutesleft, string reason)
        {
            _num = num;
            _guidIp = guidIp;
            _minutesleft = minutesleft;
            _reason = reason;
        }

        public int Num
        {
            get { return _num; }
        }

        public string GuidIp
        {
            get { return _guidIp; }
        }

        public int Minutesleft
        {
            get { return _minutesleft; }
        }

        public string Reason
        {
            get { return _reason; }
        }

        protected bool Equals(Ban other)
        {
            return _num == other._num && string.Equals(_guidIp, other._guidIp) && string.Equals(_reason, other._reason);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = _num;
                hashCode = (hashCode*397) ^ (_guidIp != null ? _guidIp.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ _minutesleft;
                hashCode = (hashCode*397) ^ (_reason != null ? _reason.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static Ban Parse(string input)
        {
            try
            {
                var regex = new Regex(@"(\d{1,3})[ ]+([^ ]+)[ ]+([^ ]+)[ ]+(.*)",
                    RegexOptions.Compiled | RegexOptions.IgnoreCase);
                var match = regex.Match(input);

                if (match.Success || match.Groups.Count != 6)
                {
                    var num = int.Parse(match.Groups[1].Value);
                    var guid = match.Groups[2].Value;

                    if (guid.Length != 32) return null;

                    var l = match.Groups[3].Value;
                    var left = l == "perm" ? 0 : (l == "-" ? -1 : int.Parse(l));

                    var reason = match.Groups[4].Value;

                    return new Ban(num, guid, left, reason);
                }

                return null;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public override string ToString()
        {
            return string.Format("NUM {0}\tGUIDIP {1}\tML {2}\tREASON {3}", Num, GuidIp, Minutesleft, Reason);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Ban) obj);
        }
    }
}