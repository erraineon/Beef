using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BabyGame;

public class KissService(
    IOptionsSnapshot<IBabyGameConfiguration> configuration,
    IBabyGameRepository babyGameRepository,
    IBabyGameLogger logger,
    ITimeProvider timeProvider)
{
    public async Task KissAsync(ulong userId)
    {
        var marriage = await GetMarriageOrThrowAsync(userId);
        EnsureKissCooldownExpired(marriage);
        var kisses = GetKisses(marriage);
    }

    private double GetKisses(Marriage marriage)
    {
        var kisses = 0.0;
        var kissAdder = GetUndergraduateBabies<IKissAdder>(marriage);
        foreach (var babyGroup in kissAdder)
        {
            var firstBaby = babyGroup.First();
            var collection = babyGroup.Cast<Baby>().ToList();
            var kissesByGroup = firstBaby.GetKisses(collection);
            kisses += kissesByGroup;
            logger.Log($"{GetBabyNames(collection)} earned you {kissesByGroup} CHU");
        }

        var kissMultiplier = 1.0;
        var kissMultipliers = GetUndergraduateBabies<IKissMultiplier>(marriage);
        foreach (var babyGroup in kissMultipliers)
        {
            var firstBaby = babyGroup.First();
            var collection = babyGroup.Cast<Baby>().ToList();
            var multiplierByGroup = firstBaby.GetKissMultiplier(collection);
            kissMultiplier += multiplierByGroup;
            logger.Log($"{GetBabyNames(collection)} earned you extra {multiplierByGroup:P} CHU");
        }

        return kisses * kissMultiplier;
    }

    private static string GetBabyNames(List<Baby> collection)
    {
        return string.Join(", ", collection.Select(x => x.Name));
    }

    private static List<IGrouping<Type, TBaby>> GetUndergraduateBabies<TBaby>(Marriage marriage)
    {
        var kissEffectors = marriage.Babies
            .Where(x => x.GraduationDate == null)
            .OfType<TBaby>()
            .GroupBy(x => x!.GetType())
            .ToList();
        return kissEffectors;
    }

    private void EnsureKissCooldownExpired(Marriage marriage)
    {
        var now = timeProvider.Now;
        var cooldown = GetKissCooldown(marriage);
        if (!marriage.SkipNextCooldown && now - marriage.LastKissedAt < cooldown)
            throw new KissOnCooldownException(cooldown);
    }

    private TimeSpan GetKissCooldown(Marriage marriage)
    {
        var kissCooldownSeconds = Math.Max(
            configuration.Value.MinimumKissCooldown.TotalSeconds,
            (configuration.Value.BaseKissCooldown *
             GetAffinityKissCooldownMultiplier(marriage) *
             GetBabiesKissCooldownMultiplier(marriage)).TotalSeconds
        );
        return TimeSpan.FromSeconds(kissCooldownSeconds);
    }

    private double GetBabiesKissCooldownMultiplier(Marriage marriage)
    {
        var kissCooldownEffectors = marriage.Babies
            .Where(x => x.GraduationDate == null)
            .OfType<IKissCooldownEffector>()
            .GroupBy(x => x.GetType())
            .ToList();
        var multiplier = 1.0;
        foreach (var babyGroup in kissCooldownEffectors)
        {
            var firstBaby = babyGroup.First();
            multiplier -= firstBaby.GetCooldownMultiplierDeduction(babyGroup.Cast<Baby>().ToList());
        }

        return multiplier;
    }

    private double GetAffinityKissCooldownMultiplier(Marriage marriage)
    {
        // f(x)=1-a+\frac{a}{1+b\cdot\frac{x}{1000}}
        // At affinity 30, 90, 360, 100: 0.92, 0.8, 0.6, 0.4 
        var a = configuration.Value.MaxAffinityKissCooldownMultiplier;
        var b = configuration.Value.AffinityKissCooldownMultiplierRate;
        var x = marriage.Affinity;
        return 1 - a + a / (1 + b * (x / 1000));
    }

    private async Task<Marriage> GetMarriageOrThrowAsync(ulong userId)
    {
        return await babyGameRepository.GetMarriageAsync(userId) ?? throw new NotMarriedException(userId);
    }
}

public interface IBabyGameLogger
{
    void Log(string message);
}

internal interface IKissMultiplier
{
    double GetKissMultiplier(ICollection<Baby> babyGroup);
}

internal interface IKissAdder
{
    double GetKisses(ICollection<Baby> babyGroup);
}

internal class KissOnCooldownException(TimeSpan cooldown) : Exception
{
    public TimeSpan Cooldown { get; } = cooldown;
}

public interface IKissCooldownEffector
{
    double GetCooldownMultiplierDeduction(ICollection<Baby> babyGroup);
}

public interface IBabyGameConfiguration
{
    TimeSpan MinimumKissCooldown { get; set; }
    TimeSpan BaseKissCooldown { get; set; }
    double MaxAffinityKissCooldownMultiplier { get; set; }
    double AffinityKissCooldownMultiplierRate { get; set; }
}

public class BabyGameConfiguration : IBabyGameConfiguration
{
    public TimeSpan MinimumKissCooldown { get; set; } = TimeSpan.FromMinutes(1);
    public TimeSpan BaseKissCooldown { get; set; } = TimeSpan.FromMinutes(10);
    public double MaxAffinityKissCooldownMultiplier { get; set; } = 0.66;
    public double AffinityKissCooldownMultiplierRate { get; set; } = 4.5;
}

public interface ITimeProvider
{
    DateTimeOffset Now { get; }
}

public class TimeProvider : ITimeProvider
{
    public DateTimeOffset Now => DateTimeOffset.Now;
}

public class MarriageService(IBabyGameRepository babyGameRepository, ITimeProvider timeProvider)
{
    public async Task MarryAsync(ulong user1Id, ulong user2Id)
    {
        EnsureNoSelfMarriage(user1Id, user2Id);
        await EnsureNotAlreadyMarriedAsync(user1Id);
        await EnsureNotAlreadyMarriedAsync(user2Id);
        await babyGameRepository.AddMarriageAsync(
            new Marriage
            {
                User1Id = user1Id,
                User2Id = user2Id,
                MarriedAt = timeProvider.Now,
                LastKissedAt = timeProvider.Now
            }
        );
    }

    private async Task EnsureNotAlreadyMarriedAsync(ulong userId)
    {
        if (await babyGameRepository.GetMarriageAsync(userId) is { } existingMarriage)
            throw new AlreadyMarriedException(existingMarriage);
    }

    private static void EnsureNoSelfMarriage(ulong user1Id, ulong user2Id)
    {
        if (user1Id == user2Id)
            throw new SelfMarriageException(user1Id);
    }
}

public class NotMarriedException(ulong userId) : Exception
{
    public ulong UserId { get; } = userId;
}

public class Marriage
{
    public Guid Id { get; init; }
    public required ulong User1Id { get; init; }
    public required ulong User2Id { get; init; }
    public required DateTimeOffset MarriedAt { get; init; }
    public decimal Kisses { get; set; }
    public List<Baby> Babies { get; init; } = new();
    public double Affinity { get; set; }
    public required DateTimeOffset LastKissedAt { get; set; }
    public bool SkipNextCooldown { get; set; }
}

public abstract class Baby
{
    public Guid Id { get; init; }
    public required Marriage Marriage { get; set; }
    public required string Name { get; set; }
    public required double Experience { get; set; }

    /// <summary>
    ///     Between 1 and 100
    ///     Follows formula: f\left(x\right)=x^{3}
    /// </summary>
    public required double Level { get; set; }

    /// <summary>
    ///     Assigned at birth. F, E, D, C, B, A, S, SS, SSS, ⭐, ⭐⭐, ⭐⭐⭐, etc. Each can have a + or -
    /// </summary>
    public required double Rank { get; set; }

    public required DateTimeOffset BirthDate { get; init; }
    public required ulong MotherId { get; init; }
    public DateTimeOffset? GraduationDate { get; set; }
}

[Description("Kiss more often")]
public class MakeMoreLoveBaby : Baby, IKissCooldownEffector
{
    public double GetCooldownMultiplierDeduction(ICollection<Baby> babyGroup)
    {
        //f\left(x\right)=\frac{\left(xb\right)^{c}+a}{\left(xb\right)^{c}+a+100}
        var totalLevels = babyGroup.Sum(x => x.Level);
        var exp = Math.Pow(totalLevels, 1.1) + 5;
        return exp / (exp + 100);
    }
}

public interface IBabyGameRepository
{
    Task AddMarriageAsync(Marriage marriage);
    Task<Marriage?> GetMarriageAsync(ulong userId);
}

public class BabyGameDbContext : DbContext, IBabyGameRepository
{
    public DbSet<Marriage> Marriages { get; set; }

    public Task<Marriage?> GetMarriageAsync(ulong userId)
    {
        return Marriages
            .FirstOrDefaultAsync(x => x.User1Id == userId || x.User2Id == userId);
    }

    public async Task AddMarriageAsync(Marriage marriage)
    {
        await Marriages.AddAsync(marriage);
        await SaveChangesAsync();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Marriage>()
            .HasIndex(x => new { x.User1Id, x.User2Id })
            .IsUnique();

        base.OnModelCreating(modelBuilder);
    }
}

public class SelfMarriageException(ulong userId) : Exception
{
    public ulong UserId { get; } = userId;
}

public class AlreadyMarriedException(Marriage existingMarriages) : Exception
{
    public Marriage ExistingMarriages { get; } = existingMarriages;
}