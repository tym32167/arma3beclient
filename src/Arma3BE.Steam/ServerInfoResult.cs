namespace Arma3BEClient.Steam
{
    public class ServerInfoResult
    {
        public int ID;
        public byte VAC;
        public int Protocol { get; set; }
        public string Name { get; set; }
        public string Map { get; set; }
        public string Folder { get; set; }
        public string Game { get; set; }
        public byte Players { get; set; }
        public byte MaxPlayers { get; set; }
        public byte Bots { get; set; }
        public string ServerType { get; set; }
        public string Environment { get; set; }
        public byte Visibility { get; set; }
        public string Version { get; set; }
        public int Port { get; set; }

        public static ServerInfoResult Parse(byte[] data)
        {
            var parser = new ResponseParser(data);
            parser.CurrentPosition += 5; //Header
            var result = new ServerInfoResult();
            result.Protocol = parser.GetByte();
            result.Name = parser.GetStringToTermination();
            result.Map = parser.GetStringToTermination();
            result.Folder = parser.GetStringToTermination();
            result.Game = parser.GetStringToTermination();
            result.ID = parser.GetShort();
            result.Players = parser.GetByte();
            result.MaxPlayers = parser.GetByte();
            result.Bots = parser.GetByte();
            result.ServerType = parser.GetStringOfByte();
            result.Environment = parser.GetStringOfByte();
            result.Visibility = parser.GetByte();
            result.VAC = parser.GetByte();
            result.Version = parser.GetStringToTermination();

            //get EDF
            uint edf = parser.GetByte();

            if ((edf & 0x80) != 0) //has port number
            {
                result.Port = parser.GetShort();
            }

            return result;
        }
    }
}