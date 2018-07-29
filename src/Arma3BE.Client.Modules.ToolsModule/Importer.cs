using Arma3BEClient.Common.Logging;
using Arma3BEClient.Libs.Repositories.Players;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace Arma3BE.Client.Modules.ToolsModule
{
    public class Importer
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly ILog _log = LogFactory.Create(new StackTrace().GetFrame(0).GetMethod().DeclaringType);

        public Importer(IPlayerRepository playerRepository)
        {
            _playerRepository = playerRepository;
        }

        private async Task<ImportResult> Import(string fname)
        {
            var sw = Stopwatch.StartNew();
            _log.Info("Import started");

            var result = new ImportResult();

            List<PlayerXML> players;
            using (var sr = new StreamReader(fname))
            {
                var ser = new XmlSerializer(typeof(List<PlayerXML>));
                players = (List<PlayerXML>)ser.Deserialize(sr);
            }


            var db =
               (await _playerRepository.GetAllPlayersAsync().ConfigureAwait(false))
                    .GroupBy(x => x.GUID)
                    .Select(x => x.OrderByDescending(y => y.Name).First())
                    .ToDictionary(x => x.GUID);

            var toadd = new List<PlayerDto>();
            var toUpdate = new List<PlayerDto>();

            foreach (var p in players)
                if (!db.ContainsKey(p.Guid))
                {
                    toadd.Add(new PlayerDto
                    {
                        Comment = p.Comment,
                        GUID = p.Guid,
                        LastIp = p.LastIP,
                        LastSeen = p.LastSeen,
                        Name = p.Name,
                        SteamId = p.SteamId,
                        Id = Guid.NewGuid()
                    });
                }
                else
                {
                    bool updated = false;

                    var lp = db[p.Guid];
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

            result.Added = toadd.Count;
            result.Updated = toUpdate.Count;

            await _playerRepository.ImportPlayersAsync(toadd, toUpdate);

            sw.Stop();
            _log.Info($"Import takes {sw.ElapsedMilliseconds} ms. Added {result.Added}, updated {result.Updated}");
            return result;
        }

        public async void Import()
        {
            var ofd = new OpenFileDialog
            {
                DefaultExt = "*.xml",
                Filter = "*.xml|*.xml",
                Title = "Select file to import players"
            };

            var res = ofd.ShowDialog();

            if (res.HasValue && res.Value)
            {
                var result = await Import(ofd.FileName);
                MessageBox.Show($"Import finished! Added {result.Added}, updated {result.Updated}");
            }
        }

        private class ImportResult
        {
            public int Added { get; set; }
            public int Updated { get; set; }
        }
    }
}