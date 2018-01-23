using Arma3BEClient.Common.Logging;
using Arma3BEClient.Libs.Tools;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Arma3BE.Client.Modules.SteamModule.Core
{
    public class SteamService : ISteamService
    {
        private IdByHashSearcher _idByHashSearcher;
        private object _lock = new object();

        private readonly string _idsFIleName;
        private readonly string _indexFIleName;

        private bool isReady = false;

        private readonly ILog _log = LogFactory.Create(new StackTrace().GetFrame(0).GetMethod().DeclaringType);

        public SteamService(ISettingsStoreSource settingsStoreSource)
        {
            var folder = settingsStoreSource.GetSettingsStore().SteamFolder;

            if (string.IsNullOrEmpty(folder)) return;

            _idsFIleName = Path.Combine(folder, "ids.bin");
            _indexFIleName = Path.Combine(folder, "index.bin");

            isReady = File.Exists(_idsFIleName) && File.Exists(_indexFIleName);

            if (isReady)
            {
                var md5 = new StandardHashProvider();
                _idByHashSearcher = new IdByHashSearcher(_idsFIleName, _indexFIleName, md5, _log);
            }
        }

        public bool IsReady()
        {
            return isReady;
        }

        public long? GetSteamId(string hash)
        {
            if (!isReady) return null;

            if (_idByHashSearcher == null) return null;

            lock (_lock)
            {
                var source = new CancellationTokenSource();
                var result = _idByHashSearcher.Search(new[] { hash }, source.Token);
                if (result.ContainsKey(hash)) return result[hash];
            }

            return null;
        }

        public Dictionary<string, long> GetSteamIds(string[] hashes)
        {
            if (!isReady) return new Dictionary<string, long>();

            if (_idByHashSearcher == null) return new Dictionary<string, long>();

            lock (_lock)
            {
                var source = new CancellationTokenSource();
                var result = _idByHashSearcher.Search(hashes, source.Token);
                return result;
            }
        }

        public Dictionary<string, long> GetSteamIds(string[] hashes, CancellationToken token)
        {
            if (!isReady) return new Dictionary<string, long>();

            if (_idByHashSearcher == null) return new Dictionary<string, long>();

            lock (_lock)
            {
                var result = _idByHashSearcher.Search(hashes, token);
                return result;
            }
        }
    }
}