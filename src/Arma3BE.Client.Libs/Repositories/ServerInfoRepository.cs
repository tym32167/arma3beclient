using Arma3BEClient.Common.Core;
using Arma3BEClient.Common.Logging;
using Arma3BEClient.Libs.Context;
using Arma3BEClient.Libs.ModelCompact;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Arma3BEClient.Libs.Repositories
{
    public interface IServerInfoRepository
    {
        void AddOrUpdate(ServerInfoDto serverInfo);
        IEnumerable<ServerInfoDto> GetActiveServerInfo();
        IEnumerable<ServerInfoDto> GetNotActiveServerInfo();
        IEnumerable<ServerInfoDto> GetServerInfo();
        void Remove(Guid serverInfoId);
        void SetServerInfoActive(Guid serverInfoId, bool active);
    }


    public class ServerInfoRepositoryCache : IServerInfoRepository
    {
        private readonly IServerInfoRepository _infoRepository;

        private static readonly object Lock = new object();
        private readonly ILog _log = LogFactory.Create(new StackTrace().GetFrame(0).GetMethod().DeclaringType);

        private static volatile bool _validCache;
        private static volatile ConcurrentDictionary<Guid, ServerInfoDto> _cache;

        private void ResetCache()
        {
            if (_validCache) return;
            lock (Lock)
            {
                if (_validCache) return;
                _log.Info("Refresh ServerInfo cache.");
                var playersByGuidsCache = new ConcurrentDictionary<Guid, ServerInfoDto>();
                var players = _infoRepository.GetServerInfo();

                foreach (var playerDto in players)
                {
                    playersByGuidsCache.AddOrUpdate(playerDto.Id, playerDto, (key, value) => value);
                }

                _cache = playersByGuidsCache;
                _validCache = true;
            }
        }


        public ServerInfoRepositoryCache(IServerInfoRepository infoRepository)
        {
            _infoRepository = infoRepository;
            Task.Run(() => ResetCache());
        }

        public void AddOrUpdate(ServerInfoDto serverInfo)
        {
            _cache.AddOrUpdate(serverInfo.Id, serverInfo, (key, value) => value);
            _infoRepository.AddOrUpdate(serverInfo);
        }

        public IEnumerable<ServerInfoDto> GetActiveServerInfo()
        {
            return _cache.Values.Where(x => x.Active).ToArray();
        }

        public IEnumerable<ServerInfoDto> GetNotActiveServerInfo()
        {
            return _cache.Values.Where(x => !x.Active).ToArray();
        }

        public IEnumerable<ServerInfoDto> GetServerInfo()
        {
            return _cache.Values.ToArray();
        }

        public void Remove(Guid serverInfoId)
        {
            ServerInfoDto val;
            _cache.TryRemove(serverInfoId, out val);
            _infoRepository.Remove(serverInfoId);
        }

        public void SetServerInfoActive(Guid serverInfoId, bool active)
        {
            ServerInfoDto value;
            if (_cache.TryGetValue(serverInfoId, out value))
            {
                if (value.Active == active) return;
                _infoRepository.SetServerInfoActive(serverInfoId, active);
                _cache.AddOrUpdate(serverInfoId, k => value, (k, v) =>
                {
                    v.Active = active;
                    return v;
                });
            }
            else
            {
                _infoRepository.SetServerInfoActive(serverInfoId, active);
                ResetCache();
            }
        }
    }

    public class ServerInfoRepository : DisposeObject, IServerInfoRepository
    {
        public IEnumerable<ServerInfoDto> GetActiveServerInfo()
        {
            using (var dc = new Arma3BeClientContext())
            {
                return dc.ServerInfo.Where(x => x.Active).Select(ToDto).ToArray();
            }
        }

        public IEnumerable<ServerInfoDto> GetServerInfo()
        {
            using (var dc = new Arma3BeClientContext())
            {
                return dc.ServerInfo.ToList().Select(ToDto).ToArray();
            }
        }

        public void SetServerInfoActive(Guid serverInfoId, bool active)
        {
            using (var dc = new Arma3BeClientContext())
            {
                var server = dc.ServerInfo.FirstOrDefault(x => x.Id == serverInfoId);
                if (server != null)
                {
                    server.Active = active;
                    dc.SaveChanges();
                }
            }
        }

        public IEnumerable<ServerInfoDto> GetNotActiveServerInfo()
        {
            using (var dc = new Arma3BeClientContext())
            {
                return dc.ServerInfo.Where(x => !x.Active).Select(ToDto).ToArray();
            }
        }

        public void AddOrUpdate(ServerInfoDto serverInfo)
        {
            using (var dc = new Arma3BeClientContext())
            {

                dc.ServerInfo.AddOrUpdate(FromDto(serverInfo));
                dc.SaveChanges();
            }
        }

        public void Remove(Guid serverInfoId)
        {
            using (var dc = new Arma3BeClientContext())
            {
                dc.ChatLog.RemoveRange(dc.ChatLog.Where(x => x.ServerId == serverInfoId));
                dc.Bans.RemoveRange(dc.Bans.Where(x => x.ServerId == serverInfoId));
                dc.PlayerHistory.RemoveRange(dc.PlayerHistory.Where(x => x.ServerId == serverInfoId));
                dc.Admins.RemoveRange(dc.Admins.Where(x => x.ServerId == serverInfoId));

                dc.ServerInfo.RemoveRange(dc.ServerInfo.Where(x => x.Id == serverInfoId));
                dc.SaveChanges();
            }
        }

        private ServerInfoDto ToDto(ServerInfo info)
        {
            return new ServerInfoDto()
            {
                Id = info.Id,
                Host = info.Host,
                Port = info.Port,
                SteamPort = info.SteamPort,
                Password = info.Password,
                Name = info.Name,
                Active = info.Active,
            };
        }

        private ServerInfo FromDto(ServerInfoDto info)
        {
            return new ServerInfo()
            {
                Id = info.Id,
                Host = info.Host,
                Port = info.Port,
                SteamPort = info.SteamPort,
                Password = info.Password,
                Name = info.Name,
                Active = info.Active,
            };
        }
    }


    public class ServerInfoDto
    {
        public Guid Id { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public int SteamPort { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
    }
}