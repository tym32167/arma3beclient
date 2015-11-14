using System;

namespace Arma3BEClient.Steam
{
    public class ServerPlayers
    {
        public byte PlayerCount { get; set; }

        public PlayerInfo[] Players { get; set; }


        public static ServerPlayers Parse(byte[] data)
        {
            var parser = new ResponseParser(data);
            parser.CurrentPosition += 5; //Header
            var result = new ServerPlayers();


            result.PlayerCount = parser.GetByte();

            result.Players = new PlayerInfo[result.PlayerCount];

            for (var i = 0; i < result.PlayerCount; i++)
            {
                var p = new PlayerInfo();

                p.N = parser.GetByte();
                p.Name = parser.GetStringToTermination();
                p.Score = parser.GetLong();
                p.Time = TimeSpan.FromSeconds(parser.GetDouble());

                //parser.CurrentPosition+=4;

                result.Players[i] = p;


                //break;
            }


            return result;
        }

        public class PlayerInfo
        {
            public byte N { get; set; }
            public string Name { get; set; }
            public long Score { get; set; }
            public TimeSpan Time { get; set; }
        }
    }
}