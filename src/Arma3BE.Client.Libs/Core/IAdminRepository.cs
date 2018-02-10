using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Arma3BE.Server.Models;

namespace Arma3BEClient.Libs.Core
{
    public interface IAdminRepository
    {
        Task AddOrUpdateAsync(IEnumerable<Admin> admins, Guid serverId);
    }
}