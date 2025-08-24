using BabyGame.Data;
using BabyGame.Events;
using BabyGame.Exceptions;
using BabyGame.Models;
using Microsoft.Extensions.Options;

namespace BabyGame.Services;

public class KissService(
    IOptionsSnapshot<BabyGameOptions> configuration,
    IBabyGameRepository babyGameRepository,
    IBabyGameLogger logger,
    IEventDispatcher eventDispatcher,
    ITimeProvider timeProvider) : IKissService
{
    public async Task KissAsync(Player player)
    {
        // TODO: implement multi-kiss
        var marriage = await babyGameRepository.GetMarriageAsync(player);
        EnsureKissCooldownExpired(marriage);
        var chu = GetChu(marriage);
        marriage.Chu += chu;
        logger.Log($"You have earned {chu} Chu");
        eventDispatcher.Aggregate<IKissComplete, int>(marriage);
        marriage.LastKissedAt = timeProvider.Now;
        await babyGameRepository.SaveMarriageAsync(marriage);
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
            .LogByType(logger, (x, y) => $"{x} earned you {chu * y} extra Chu")
            .Sum();

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
             (1 - eventDispatcher
                 .Aggregate<IKissCooldownMultiplierOnKiss, double>(marriage)
                 .Sum())).TotalSeconds
        );
        return TimeSpan.FromSeconds(kissCooldownSeconds);
    }
}