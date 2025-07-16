using System.Reflection;
using BabyGame.Attributes;
using BabyGame.Data;
using BabyGame.Extensions;
using BabyGame.Models;

namespace BabyGame.Services;

public class BabyGachaService(
    IRandomProvider randomProvider,
    ITimeProvider timeProvider,
    IBabyGameLogger logger) : IBabyGachaService
{
    public Baby CreateBaby(Marriage marriage, string? babyName = null)
    {
        var baby = InstantiateRandomBaby(marriage);
        baby.BirthDate = timeProvider.Now;
        baby.Name = babyName ?? $"Baby{marriage.Babies.Count + 1}";
        marriage.Babies.Add(baby);
        logger.Log($"Mr. Stork delivered {baby.Name}, the {baby.GetTypeName()} (Rank {baby.GetRank()})");
        return baby;
    }   

    private Baby InstantiateRandomBaby(Marriage marriage)
    {
        var babyType = GetRandomBabyType(marriage);
        var baby = InstantiateBaby(babyType);
        return baby;
    }

    private Type GetRandomBabyType(Marriage marriage)
    {
        var rarity = GetRandomRarity(marriage, out var resetPity);
        if (resetPity)
            marriage.Pity = 0;

        var eligibleTypes = typeof(Baby).Assembly.GetTypes()
            .Where(x => x.IsSubclassOf(typeof(Baby)))
            .Select(x => (
                    type: x,
                    rarity: GetBabyRarity(x)
                )
            )
            .Where(t => Math.Abs(t.rarity - rarity) < double.Epsilon)
            .Select(t => t.type)
            .ToArray();

        var babyType = randomProvider.NextItem(marriage, eligibleTypes);
        return babyType;
    }

    public double GetBabyRarity(Type babyType)
    {
        return babyType.GetCustomAttribute<RarityAttribute>()?.Rarity ?? BabyRarities.Common;
    }

    public double GetRandomRarity(Marriage marriage, out bool resetPity)
    {
        // TODO: rarity modifiers here
        var rarities = new[] { BabyRarities.Common, BabyRarities.Rare, BabyRarities.SuperRare, BabyRarities.Legendary };
        var accumulatedRarities = rarities
            .Select((x, i) => (threshold: rarities.Take(i).Sum(), value: x))
            .ToList();

        resetPity = false;
        var pityTargetThreshold = accumulatedRarities
            .First(x => Math.Abs(x.value - BabyRarities.Legendary) < double.Epsilon).threshold;
        var roll = randomProvider.NextDouble(marriage);
        if (roll < pityTargetThreshold && roll + marriage.Pity >= pityTargetThreshold)
        {
            roll += marriage.Pity;
            resetPity = true;
        }

        var rarity = accumulatedRarities
            .AsEnumerable()
            .Reverse()
            .First(x => roll >= x.threshold);
        return rarity.value;
    }

    private Baby InstantiateBaby(Type type)
    {
        var baby = (Baby)GetType()
            .GetMethod(nameof(InstantiateBabyGeneric), BindingFlags.Instance | BindingFlags.NonPublic)
            .MakeGenericMethod(type).Invoke(this, null);
        return baby!;
    }

    private TBaby InstantiateBabyGeneric<TBaby>() where TBaby : Baby, new()
    {
        return new TBaby();
    }
}