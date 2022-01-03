using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Beef.Core.Data;

public interface IBeefDbContext
{
    DbSet<GuildOptionsEntity> Guilds { get; set; }
    DatabaseFacade Database { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}