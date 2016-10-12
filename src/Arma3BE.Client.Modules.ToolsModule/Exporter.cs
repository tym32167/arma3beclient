using Arma3BE.Client.Modules.MainModule.Models.Export;
using Arma3BEClient.Libs.Repositories;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Arma3BE.Client.Modules.ToolsModule
{
    public class Exporter
    {
        private readonly IPlayerRepository _playerRepository;

        public Exporter(IPlayerRepository playerRepository)
        {
            _playerRepository = playerRepository;
        }

        public void Export(string fname)
        {
            var list =
                _playerRepository.GetAllPlayers()
                    .GroupBy(x => x.GUID)
                    .Select(x => x.OrderByDescending(y => y.Name).First())
                    .OrderBy(x => x.Name)
                    .Select(x =>
                        new PlayerXML
                        {
                            Guid = x.GUID,
                            SteamId = x.SteamId,
                            LastIP = x.LastIp,
                            LastSeen = x.LastSeen,
                            Name = x.Name,
                            Comment = x.Comment
                        }).ToList();


            using (var sw = new StreamWriter(fname))
            {
                var ser = new XmlSerializer(typeof(List<PlayerXML>));
                ser.Serialize(sw, list);
            }
        }
    }
}