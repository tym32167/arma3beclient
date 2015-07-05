using System.Linq;
using Arma3BEClient.Libs.ModelCompact;

namespace Arma3BEClient.Libs.Context
{
    public interface IContext
    {
        IQueryable<ChatLog> ChatLog { get;  }
        IQueryable<Note> Comments { get;  }
        IQueryable<Player> Player { get;  }
        IQueryable<ServerInfo> ServerInfo { get;  }
        IQueryable<Settings> Settings { get;  }
        IQueryable<Ban> Bans { get;  }
        IQueryable<Admin> Admins { get;  }
        IQueryable<PlayerHistory> PlayerHistory { get;  }

        void Save();
    }
}