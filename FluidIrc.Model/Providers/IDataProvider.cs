using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluidIrc.Model.Models;

namespace FluidIrc.Model.Providers
{
    public interface IDataProvider
    {

        Task AddServer(Server server);

        Task RemoveServer(Guid serverId);
        Task RemoveServer(Server server);

        Task<List<Server>> GetServers();
        Task<Server> GetServer(Guid id);

        Task<List<UserProfile>> GetProfiles();

    }
}
