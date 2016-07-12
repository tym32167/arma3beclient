using Arma3BEClient.Common.Attributes;
using System;
using System.Text.RegularExpressions;

namespace Arma3BE.Server.Models
{
    public class Admin
    {
        private static readonly Regex CompidelRegex =
            new Regex(@"(\d{1,3})[ ]+(\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}):([\d]+)",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private Admin(int num, string ip, int port)
        {
            Num = num;
            IP = ip;
            Port = port;
        }

        [ShowInUi]
        [EnableCopy]
        public int Num { get; }

        [ShowInUi]
        [EnableCopy]
        public string IP { get; }

        [ShowInUi]
        [EnableCopy]
        public int Port { get; }

        private bool Equals(Admin other)
        {
            return Num == other.Num && string.Equals(IP, other.IP) && Port == other.Port;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Num;
                hashCode = (hashCode * 397) ^ (IP?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ Port;
                return hashCode;
            }
        }

        public static Admin Parse(string input)
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
                    return new Admin(num, ip, port);
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
            return Equals((Admin)obj);
        }
    }
}