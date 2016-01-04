using System;
using System.Text.RegularExpressions;

namespace Arma3BE.Server.Messaging.Recognizers.Core
{
    public class IPAndPortValidator
    {
        public static bool Validate(string value)
        {
            if (string.IsNullOrEmpty(value)) return false;

            if (!RegexIpAndPort.IsMatch(value)) return false;
            if (value.Split(".:".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Length != 5) return false;

            return true;
        }

        private static readonly Regex RegexIpAndPort = new Regex(
            @"(\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}):([\d]+)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);
    }
}