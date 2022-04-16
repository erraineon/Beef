using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;

namespace Beef.Core.Data;



public class BeefDbContext : DbContext, IBeefDbContext
{
    public BeefDbContext(DbContextOptions<BeefDbContext> options) : base(options)
    {
    }

    public DbSet<GuildOptionsEntity> Guilds { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GuildOptionsEntity>(
            e =>
            {
                e.Property(x => x.Id).ValueGeneratedNever();
                HasJsonConversion(e.Property(x => x.Value));
            }
        );
    }

    private static void HasJsonConversion<T>(PropertyBuilder<T> propertyBuilder) where T : class?, new()
    {
        var settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Include,
            TypeNameHandling = TypeNameHandling.Auto
        };

        var converter = new ValueConverter<T, string?>(
            v => TrySerializeOrNull(v, settings),
            v => TryDeserializeOrNew<T>(v, settings)
        );

        var comparer = new ValueComparer<T>(
            (l, r) => TrySerializeOrNull(l, settings) == TrySerializeOrNull(r, settings),
            v => v == null ? 0 : TrySerializeOrNull(v, settings).GetHashCode(),
            v => TryDeserializeOrNew<T>(
                TrySerializeOrNull(v, settings),
                settings
            )
        );

        propertyBuilder.HasConversion(converter);
        propertyBuilder.Metadata.SetValueConverter(converter);
        propertyBuilder.Metadata.SetValueComparer(comparer);
    }

    [return: NotNullIfNotNull("v")]
    private static string? TrySerializeOrNull<T>(T? v, JsonSerializerSettings settings)
        where T : class?, new()
    {
        return v != null ? JsonConvert.SerializeObject(v, settings) : null;
    }

    private static T TryDeserializeOrNew<T>(string? v, JsonSerializerSettings jsonSerializerSettings)
        where T : class?, new()
    {
        return (v != null ? JsonConvert.DeserializeObject<T>(v, jsonSerializerSettings) : null) ?? new T();
    }
}