using Arma3BEClient.Common.Logging;
using Arma3BEClient.Libs.Tools;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Arma3BEClient.Libs.Core.Settings;

namespace Arma3BE.Client.Modules.SteamModule.Core
{
    public class SteamService : ISteamService
    {
        private readonly IdByHashSearcher _idByHashSearcher;
        private readonly object _lock = new object();

        private readonly ConcurrentDictionary<string, string> _failedSearches = new ConcurrentDictionary<string, string>();

        private readonly bool _isReady;

        private readonly ILog _log = LogFactory.Create(new StackTrace().GetFrame(0).GetMethod().DeclaringType);

        public SteamService(ISettingsStoreSource settingsStoreSource)
        {
            var folder = settingsStoreSource.GetSettingsStore().SteamFolder;

            if (string.IsNullOrEmpty(folder)) return;

            var idsFIleName = Path.Combine(folder, "ids.bin");
            var indexFIleName = Path.Combine(folder, "index.bin");

            _isReady = File.Exists(idsFIleName) && File.Exists(indexFIleName);

            if (_isReady)
            {
                var md5 = new StandardHashProvider();
                _idByHashSearcher = new IdByHashSearcher(idsFIleName, indexFIleName, md5, _log);
            }
        }

        public bool IsReady()
        {
            return _isReady;
        }

        public long? GetSteamId(string hash)
        {
            if (_failedSearches.ContainsKey(hash)) return null;

            if (!_isReady) return null;

            if (_idByHashSearcher == null) return null;

            lock (_lock)
            {
                var source = new CancellationTokenSource();
                var result = _idByHashSearcher.Search(new[] { hash }, source.Token);
                if (result.ContainsKey(hash)) return result[hash];
            }

            _failedSearches.AddOrUpdate(hash, x => hash, (x, y) => hash);
            return null;
        }

        public Dictionary<string, long> GetSteamIds(string[] hashes)
        {
            hashes = hashes.Where(x => !_failedSearches.ContainsKey(x)).ToArray();

            if (!hashes.Any()) return new Dictionary<string, long>();
            if (!_isReady) return new Dictionary<string, long>();

            if (_idByHashSearcher == null) return new Dictionary<string, long>();

            lock (_lock)
            {
                var source = new CancellationTokenSource();
                var result = _idByHashSearcher.Search(hashes, source.Token);

                var notFound = hashes.Where(x => !result.ContainsKey(x));
                foreach (var f in notFound)
                    _failedSearches.AddOrUpdate(f, x => f, (x, y) => f);

                return result;
            }
        }

        public Dictionary<string, long> GetSteamIds(string[] hashes, CancellationToken token)
        {
            hashes = hashes.Where(x => !_failedSearches.ContainsKey(x)).ToArray();

            if (!hashes.Any()) return new Dictionary<string, long>();

            if (!_isReady) return new Dictionary<string, long>();

            if (_idByHashSearcher == null) return new Dictionary<string, long>();

            lock (_lock)
            {
                var result = _idByHashSearcher.Search(hashes, token);

                var notFound = hashes.Where(x => !result.ContainsKey(x));
                foreach (var f in notFound)
                    _failedSearches.AddOrUpdate(f, x => f, (x, y) => f);

                return result;
            }
        }
    }
}