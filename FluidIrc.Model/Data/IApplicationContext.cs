using System;
using System.Threading;
using System.Threading.Tasks;
using FluidIrc.Model.Models;
using Microsoft.EntityFrameworkCore;

namespace FluidIrc.Model.Data
{
    public interface IApplicationContext : IDisposable
    {

        DbSet<UserProfile> UserProfiles { get; set; }
        DbSet<Server> Servers { get; set; }

        int SaveChanges();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));

    }
}
