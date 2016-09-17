using System;

namespace Arma3BE.Server.Models
{
    public class Mission
    {
        private Mission(string name)
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
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}