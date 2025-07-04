using BabyGame.Data;
using BabyGame.Events;
using BabyGame.Exceptions;
using Microsoft.Extensions.Options;

namespace BabyGame.Services;

public class KissService(
    IOptionsSnapshot<IBabyGameConfiguration> configuration,
    IModifierService modifierService,
    IBabyGameRepository babyGameRepository,
    IBabyGameLogger logger,
    IEventDispatcher eventDispatcher,
    ITimeProvider timeProvider)
{
    public async Task KissAsync(Player player)
    {
        await using var transaction = await babyGameRepository.BeginTransactionAsync();
        // TODO: implement multi-kiss
        var marriage = await babyGameRepository.GetMarriageAsync(player);
        EnsureKissCooldownExpired(marriage);
        var chu = GetChu(marriage);
        marriage.Chu += chu;
        logger.Log($"You have earned {chu} Chu");
        var now = timeProvider.Now;
        var firstKiss = marriage.LastKissedAt == null;
        if (firstKiss || now - marriage.LastKissedAt >= TimeSpan.FromDays(1)) marriage.Affinity++;
        marriage.LastKissedAt = now;
        await babyGameRepository.SaveChangesAsync();
    }

    private decimal GetChu(Marriage marriage)
    {
        var chu = 0.0;
        chu += eventDispatcher
            .Aggregate<IChuOnKiss, double>(marriage)
            .LogByType(logger, "{0} earned you {1} Chu")
            .Sum();

        var chuMultiplier = 1.0;
        chuMultiplier += eventDispatcher
            .Aggregate<IChuMultiplierOnKiss, double>(marriage)
            .LogByType(logger, (x,y) => $"{x} earned you {chu * y} extra Chu")
            .Sum();

        var affinityBonus = marriage.Affinity / 100.0;
        chuMultiplier += affinityBonus;

        return (decimal)Math.Max(0, chu * chuMultiplier);
    }

    private void EnsureKissCooldownExpired(Marriage marriage)
    {
        var now = timeProvider.Now;
        var cooldown = GetKissCooldown(marriage);
        if (now - marriage.LastKissedAt < cooldown)
            throw new KissOnCooldownException(cooldown);
    }

    private TimeSpan GetKissCooldown(Marriage marriage)
    {
        var kissCooldownSeconds = Math.Max(
            configuration.Value.MinimumKissCooldown.TotalSeconds,
            (configuration.Value.BaseKissCooldown *
             GetAffinityKissCooldownMultiplier(marriage) *
             eventDispatcher
                 .Aggregate<IKissCooldownMultiplierOnKiss, double>(marriage)
                 .Sum()).TotalSeconds
        );
        return TimeSpan.FromSeconds(kissCooldownSeconds);
    }

    private double GetAffinityKissCooldownMultiplier(Marriage marriage)
    {
        // f(x)=1-a+\frac{a}{1+b\cdot\frac{x}{1000}}
        // At affinity 30, 90, 360, 100: 0.92, 0.8, 0.6, 0.4 
        var a = configuration.Value.MaxAffinityKissCooldownMultiplier;
        var b = configuration.Value.AffinityKissCooldownMultiplierRate;
        var x = marriage.Affinity;
        return 1 - a + a / (1 + b * (x / 1000.0));
    }
}