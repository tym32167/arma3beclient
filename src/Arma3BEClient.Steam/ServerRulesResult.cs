using System.Collections.Generic;

namespace Arma3BEClient.Steam
{
    public class ServerRulesResult : Dictionary<string, string>
    {
        public static ServerRulesResult Parse(byte[] bytes)
        {
            var result = new ServerRulesResult();
            var parser = new ResponseParser(bytes);
            parser.CurrentPosition += 7;
            while (parser.BytesLeft)
            {
                result.Add(parser.GetStringToTermination(), parser.GetStringToTermination());
            }
            return result;
        }
    }
}