using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Arma3BEClient.Libs.Tools
{
    public interface ISteamService
    {
        bool IsReady();
        long? GetSteamId(string hash);
        Dictionary<string, long> GetSteamIds(string[] hashes);
        Dictionary<string, long> GetSteamIds(string[] hashes, CancellationToken token);
    }
}