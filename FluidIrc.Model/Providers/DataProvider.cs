using FluidIrc.Model.Data;
using FluidIrc.Model.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FluidIrc.Model.Providers
{
    public class DataProvider : IDataProvider
    {

        private readonly IApplicationContextFactory _appContextFactory;

        public DataProvider(IApplicationContextFactory appContextFactory)
        {
            _appContextFactory = appContextFactory;
        }

        public async Task AddServer(Server server)
        {
            using (var context = _appContextFactory.CreateApplicationContext())
            {
                if (await context.Servers.FindAsync(server.Id) == null)
                {
                    await context.Servers.AddAsync(server);
                    await context.SaveChangesAsync();
                }
            }
        }

        public async Task RemoveServer(Guid serverId)
        {
            using (var context = _appContextFactory.CreateApplicationContext())
            {
                var server = await context.Servers.FindAsync(serverId);
                if (server != null)
                {
                    context.Servers.Remove(server);
                    await context.SaveChangesAsync();
                }
            }
        }

        public Task RemoveServer(Server server)
        {
            return RemoveServer(server.Id);
        }

        public async Task<List<Server>> GetServers()
        {
            using (var context = _appContextFactory.CreateApplicationContext())
            {
                return await context.Servers.Include(s => s.UserProfile).ToListAsync();
            }
        }

        public async Task<Server> GetServer(Guid id)
        {
            using (var context = _appContextFactory.CreateApplicationContext())
            {
                return await context.Servers.Include(s => s.UserProfile).FirstOrDefaultAsync(s => s.Id.Equals(id));
            }
        }

        public async Task<List<UserProfile>> GetProfiles()
        {
            using (var context = _appContextFactory.CreateApplicationContext())
            {
                return await context.UserProfiles.ToListAsync();
            }
        }
    }
}
