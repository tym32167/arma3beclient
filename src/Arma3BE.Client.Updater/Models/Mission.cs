using System;

namespace Arma3BEClient.Updater.Models
{
    public class Mission
    {
        public Mission(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public static Mission Parse(string input)
        {
            try
            {
                if (input == "Missions on server:") return null;
                return new Mission(input);
/*
                var regex = new Regex(@"(\d{1,3})[ ]+(\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}):([\d]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                var match = regex.Match(input);

                if (match.Success || match.Groups.Count != 6)
                {
                    var num = Int32.Parse(match.Groups[1].Value);
                    var ip = match.Groups[2].Value;
                    var port = Int32.Parse(match.Groups[3].Value);
                    return new Admin(num, ip, port);
                }*/

                return null;
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}