using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Arma3BEClient.Common.Core;
using Arma3BEClient.Common.Logging;
using Arma3BEClient.Libs.Core;
using Arma3BEClient.Libs.EF.Context;
using Arma3BEClient.Libs.EF.Model;

namespace Arma3BEClient.Libs.EF.Repositories
{
    public class ServerInfoRepositoryCache : IServerInfoRepository
    {
        private readonly IServerInfoRepository _infoRepository;

        private static readonly object Lock = new object();
        private readonly ILog _log = LogFactory.Create(new StackTrace().GetFrame(0).GetMethod().DeclaringType);

        private static volatile bool _validCache;
        private static volatile ConcurrentDictionary<Guid, ServerInfoDto> _cache;

        private Task ResetCacheAsync()
        {
            return Task.Run(() => { ResetCache(); });
        }

        async Task<ConcurrentDictionary<Guid, ServerInfoDto>> GetCacheAsync()
        {
            if (!_validCache) await ResetCacheAsync();
            return _cache;
        }

        private void ResetCache()
        {
            if (_validCache) return;
            lock (Lock)
            {
                if (_validCache) return;
                _log.Info("Refresh ServerInfo cache.");
                var playersByGuidsCache = new ConcurrentDictionary<Guid, ServerInfoDto>();
                var players = _infoRepository.GetServerInfoAsync().Result;

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
        }

        public async Task AddOrUpdateAsync(ServerInfoDto serverInfo)
        {
            (await GetCacheAsync()).AddOrUpdate(serverInfo.Id, serverInfo, (key, value) => value);
            await _infoRepository.AddOrUpdateAsync(serverInfo);
        }

        public async Task<IEnumerable<ServerInfoDto>> GetActiveServerInfoAsync()
        {
            return (await GetCacheAsync()).Values.Where(x => x.Active).ToArray();
        }

        public async Task<IEnumerable<ServerInfoDto>> GetNotActiveServerInfoAsync()
        {
            return (await GetCacheAsync()).Values.Where(x => !x.Active).ToArray();
        }

        public async Task<IEnumerable<ServerInfoDto>> GetServerInfoAsync()
        {
            return (await GetCacheAsync()).Values.ToArray();
        }

        public async Task RemoveAsync(Guid serverInfoId)
        {
            ServerInfoDto val;
            (await GetCacheAsync()).TryRemove(serverInfoId, out val);
            await _infoRepository.RemoveAsync(serverInfoId);
        }

        public async Task SetServerInfoActiveAsync(Guid serverInfoId, bool active)
        {
            ServerInfoDto value;
            if ((await GetCacheAsync()).TryGetValue(serverInfoId, out value))
            {
                if (value.Active == active) return;
                await _infoRepository.SetServerInfoActiveAsync(serverInfoId, active);
                (await GetCacheAsync()).AddOrUpdate(serverInfoId, k => value, (k, v) =>
                {
                    v.Active = active;
                    return v;
                });
            }
            else
            {
                await _infoRepository.SetServerInfoActiveAsync(serverInfoId, active);
                ResetCache();
            }
        }
    }

    public class ServerInfoRepository : DisposeObject, IServerInfoRepository
    {
        public async Task<IEnumerable<ServerInfoDto>> GetActiveServerInfoAsync()
        {
            return await Task.Run(() =>
            {
                using (var dc = new Arma3BeClientContext())
                {
                    return dc.ServerInfo.Where(x => x.Active).Select(ToDto).ToArray();
                }
            });
        }

        public async Task<IEnumerable<ServerInfoDto>> GetServerInfoAsync()
        {
            return await Task.Run(() =>
            {
                using (var dc = new Arma3BeClientContext())
                {
                    return dc.ServerInfo.ToList().Select(ToDto).ToArray();
                }
            });
        }

        public Task SetServerInfoActiveAsync(Guid serverInfoId, bool active)
        {
            return Task.Run(() =>
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
            });
        }

        public async Task<IEnumerable<ServerInfoDto>> GetNotActiveServerInfoAsync()
        {
            return await Task.Run(() =>
            {
                using (var dc = new Arma3BeClientContext())
                {
                    return dc.ServerInfo.Where(x => !x.Active).Select(ToDto).ToArray();
                }
            });
        }

        public Task AddOrUpdateAsync(ServerInfoDto serverInfo)
        {
            return Task.Run(() =>
            {
                using (var dc = new Arma3BeClientContext())
                {

                    dc.ServerInfo.AddOrUpdate(FromDto(serverInfo));
                    dc.SaveChanges();
                }
            });
        }

        public Task RemoveAsync(Guid serverInfoId)
        {
            return Task.Run(() =>
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
            });
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