using Arma3BEClient.Common.Extensions;
using Arma3BEClient.Common.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Arma3BE.Client.Modules.SteamModule.Core
{
    public class IdByHashSearcher
    {
        private readonly string _idsFile;
        private readonly IMd5Provider _md5Provider;
        private readonly ILog _log;
        private readonly Lazy<uint[]> _cache;

        public IdByHashSearcher(string idsFile, string indexFile, IMd5Provider md5Provider, ILog log)
        {
            _idsFile = idsFile;
            _md5Provider = md5Provider;
            _log = log;

            _cache = new Lazy<uint[]>(() =>
            {
                using (_log.Time("Creating cache"))
                {
                    return GetCache(indexFile);
                }
            });
        }

        // 70473 - 6:13 vs 1:15
        public Dictionary<string, long> Search(string[] ids, CancellationToken token)
        {
            var filteredIds = ids.Where(x => x.IsGuid()).Distinct().ToArray();

            var idsHash = new HashSet<string>(filteredIds);
            var result = new Dictionary<string, long>();

            _log.Info($"Given ids:{ids.Length}, Filtered ids:{filteredIds.Length}, Not correct ids:{ids.Length - filteredIds.Length}");

            using (_log.Time($"Searching for {filteredIds.Length} ids"))
            {
                using (var sr = _idsFile.CreateReader().Buffered(4 * 400).ToBinaryReader())
                {
                    foreach (var id in filteredIds)
                    {
                        Search(id, _cache.Value, sr, idsHash, result);
                        if (!idsHash.Any()) break;
                        if (token.IsCancellationRequested) return result;
                    }
                }
                _log.Info($"Found {result.Count} ids");
            }

            return result;
        }

        long lmin = 0x0110000100000000;
        private void Search(string shash, uint[] cache, BinaryReader reader, HashSet<string> idsHashSet, Dictionary<string, long> result)
        {
            var hash = FromString(shash);
            var ind = hash[0] * 256 * 256 + hash[1] * 256 + hash[2];
            var startLine = cache[ind];
            var endLine = uint.MaxValue;
            if (ind < cache.Length - 1)
                endLine = cache[ind + 1];

            reader.BaseStream.Position = startLine * 4L;

            for (var i = startLine; i < endLine; i++)
            {
                var id = reader.ReadUInt32();
                var ui = id + lmin;
                var midhash = _md5Provider.ComputeByteHash(id);

                var str = ToString(midhash);

                if (idsHashSet.Contains(str))
                {
                    result.Add(str, ui);
                    idsHashSet.Remove(str);
                }
            }
        }

        private static byte[] FromString(String hex)
        {
            var numberChars = hex.Length;
            var bytes = new byte[numberChars / 2];
            for (var i = 0; i < numberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        private static string ToString(byte[] hash)
        {
            var result = "";
            foreach (var b in hash)
                result += b.ToString("x2");

            return result;
        }

        private static uint[] GetCache(string indexFile)
        {
            var cache = new uint[256 * 256 * 256];

            using (var reader = indexFile.CreateReader().Buffered(10 * 1024 * 1024).ToBinaryReader())
            {
                for (var i = 0; i < 256 * 256 * 256; i++)
                {
                    cache[i] = reader.ReadUInt32();
                }
            }

            return cache;
        }
    }
}