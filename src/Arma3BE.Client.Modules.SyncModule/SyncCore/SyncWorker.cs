using Arma3BEClient.Libs.Repositories.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Arma3BE.Client.Modules.SyncModule.SyncCore
{
    public class SyncWorker
    {
        private readonly IPlayerRepository _playerRepository;

        public SyncWorker(IPlayerRepository playerRepository)
        {
            _playerRepository = playerRepository;
        }

        public async Task<ImportResult> Sync(SyncCredentials credentials, CancellationToken cancellationToken, IProgress<int> progress)
        {
            var backend = new BackendClient(credentials, new HttpGenericClient());
            var total = 0;
            var count = 1000;
            var offset = 0;

            var result = new ImportResult();

            var db =
                (await _playerRepository.GetAllPlayersAsync())
                .GroupBy(x => x.GUID)
                .Select(x => x.OrderByDescending(y => y.Name).First())
                .ToDictionary(x => x.GUID);

            do
            {
                var resp = await backend.GetPlayers(offset, count);
                total = resp.Count;
                var players = resp.Players;

                if (resp.Players.Length == 0 || offset > total) break;

                offset += count;

                var toadd = new List<PlayerDto>();
                var toUpdate = new List<PlayerDto>();


                foreach (var p in players)
                    if (!db.ContainsKey(p.GUID))
                    {
                        toadd.Add(new PlayerDto
                        {
                            Comment = p.Comment,
                            GUID = p.GUID,
                            LastIp = p.IP,
                            LastSeen = p.LastSeen,
                            Name = p.Name,
                            SteamId = p.SteamId,
                            Id = Guid.NewGuid()
                        });
                    }
                    else
                    {
                        bool updated = false;

                        var lp = db[p.GUID];
                        if (string.IsNullOrEmpty(lp.Comment) && !string.IsNullOrEmpty(p.Comment))
                        {
                            lp.Comment = p.Comment;
                            updated = true;
                        }
                        if (string.IsNullOrEmpty(lp.SteamId) && !string.IsNullOrEmpty(p.SteamId))
                        {
                            lp.SteamId = p.SteamId;
                            updated = true;
                        }
                        if (updated)
                        {
                            toUpdate.Add(lp);
                        }
                    }

                await _playerRepository.ImportPlayersAsync(toadd, toUpdate);

                result.Added += toadd.Count;
                result.Updated += toUpdate.Count;


                if (total != 0)
                {
                    var percentage = 100 * (offset + count) / total;
                    progress.Report(percentage);
                }


            } while (!cancellationToken.IsCancellationRequested);

            return result;
        }
    }

    public class ImportResult
    {
        public int Added { get; set; }
        public int Updated { get; set; }
    }
}