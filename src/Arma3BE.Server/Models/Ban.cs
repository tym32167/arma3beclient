using System;
using System.Text.RegularExpressions;

namespace Arma3BE.Server.Models
{
    public class Ban
    {
        private static readonly Regex CompidelRegex = new Regex(@"(\d{1,5})[ ]+([^ ]+)[ ]+([^ ]+)[ ]+(.*)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private Ban(int num, string guidIp, int minutesleft, string reason)
        {
            Num = num;
            GuidIp = guidIp;
            Minutesleft = minutesleft;
            Reason = reason;
        }

        public int Num { get; }

        public string GuidIp { get; }

        public int Minutesleft { get; }

        public string Reason { get; }

        private bool Equals(Ban other)
        {
            return Num == other.Num && string.Equals(GuidIp, other.GuidIp) && string.Equals(Reason, other.Reason);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Num;
                hashCode = (hashCode * 397) ^ (GuidIp?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ Minutesleft;
                hashCode = (hashCode * 397) ^ (Reason?.GetHashCode() ?? 0);
                return hashCode;
            }
        }

        public static Ban Parse(string input)
        {
            try
            {
                var match = CompidelRegex.Match(input);

                if (match.Success || match.Groups.Count != 6)
                {
                    int num;

                    if (!int.TryParse(match.Groups[1].Value, out num)) return null;

                    var guid = match.Groups[2].Value;

                    if (guid.Length != 32) return null;

                    var l = match.Groups[3].Value;

                    var left = l == "perm" ? 0 : (l == "-" ? -1 : -2);
                    if (left == -2)
                    {
                        if (!int.TryParse(l, out left)) return null;
                    }

                    var reason = match.Groups[4].Value;

                    return new Ban(num, guid, left, reason);
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public override string ToString()
        {
            return $"NUM {Num}\tGUIDIP {GuidIp}\tML {Minutesleft}\tREASON {Reason}";
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Ban)obj);
        }
    }
}