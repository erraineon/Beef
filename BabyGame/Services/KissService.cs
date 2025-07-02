using BabyGame.Data;
using BabyGame.Exceptions;
using Humanizer;
using Microsoft.Extensions.Options;

namespace BabyGame.Services;

public class KissService(
    IOptionsSnapshot<IBabyGameConfiguration> configuration,
    IBabyGameRepository babyGameRepository,
    IBabyGameLogger logger,
    ITimeProvider timeProvider)
{
    public async Task KissAsync(ulong userId)
    {
        var marriage = await babyGameRepository.GetMarriageAsync(userId);
        EnsureKissCooldownExpired(marriage);
        var chu = GetChu(marriage);
        marriage.Chu += chu;
        logger.Log($"You have earned {chu} Chu");
        var now = timeProvider.Now;
        if (now - marriage.LastKissedAt >= TimeSpan.FromDays(1)) marriage.Affinity++;
        marriage.LastKissedAt = now;
    }

    private decimal GetChu(Marriage marriage)
    {
        var chu = 0.0;
        var chuAdders = GetUndergraduateBabies<IChuAdder>(marriage);
        foreach (var babyGroup in chuAdders)
        {
            var firstBaby = babyGroup.First();
            var collection = babyGroup.Cast<Baby>().ToList();
            var kissesByGroup = firstBaby.GetChu(collection);
            chu += kissesByGroup;
            logger.Log($"{GetBabyNames(collection)} earned you {kissesByGroup} Chu");
        }

        var chuMultiplier = 1.0;
        var chuMultipliers = GetUndergraduateBabies<IKissMultiplier>(marriage);
        foreach (var babyGroup in chuMultipliers)
        {
            var firstBaby = babyGroup.First();
            var collection = babyGroup.Cast<Baby>().ToList();
            var multiplierByGroup = firstBaby.GetKissMultiplier(collection);
            chuMultiplier += multiplierByGroup;
            logger.Log($"{GetBabyNames(collection)} earned you {chu * multiplierByGroup} extra Chu");
        }

        var affinityBonus = marriage.Affinity / 100.0;
        chuMultiplier += affinityBonus;

        return (decimal)Math.Max(0, chu * chuMultiplier);
    }

    private static string GetBabyNames(List<Baby> collection)
    {
        return collection.Select(x => x.Name).Humanize();
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
        return 1 - a + a / (1 + b * (x / 1000.0));
    }
}