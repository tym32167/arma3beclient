using System;
using System.Collections.Generic;

namespace Arma3BE.Client.Modules.BEServerModule
{
    public interface IBEService
    {
        IEnumerable<Guid> ConnectedServers();
    }
}