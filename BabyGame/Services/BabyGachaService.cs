using System.Reflection;
using BabyGame.Data;

namespace BabyGame.Services;

public class BabyGachaService(IRandomProvider randomProvider, IBabyGameRepository babyGameRepository, ITimeProvider timeProvider) : IBabyGachaService
{
    public async Task<Baby> CreateBabyAsync(Marriage marriage, string? babyName = null)
    {
        var baby = InstantiateRandomBaby(marriage);
        baby.BirthDate = timeProvider.Now;
        baby.Name = babyName ?? $"Baby{marriage.Babies.Count + 1}";
        marriage.Babies.Add(baby);
        await babyGameRepository.SaveChangesAsync();
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

        var eligibleTypes = typeof(Baby).Assembly.DefinedTypes
            .Where(typeof(Baby).IsAssignableFrom)
            .Select(x => (type: x.DeclaringType!,
                rarity: x.GetCustomAttribute<RarityAttribute>()?.Rarity ?? BabyRarities.Common)
            )
            .Where(t => Math.Abs(t.rarity - rarity) < double.Epsilon)
            .Select(t => t.type)
            .ToArray();

        var babyType = randomProvider.NextItem(marriage, eligibleTypes);
        return babyType;
    }

    private double GetRandomRarity(Marriage marriage, out bool resetPity)
    {
        // TODO: rarity modifiers here
        resetPity = false;
        var pityTargetRarity = BabyRarities.Legendary;
        var roll = randomProvider.NextDouble(marriage);
        if (roll < pityTargetRarity) roll = Math.Min(1, roll + marriage.Pity);
        if (roll >= pityTargetRarity)
        {
            resetPity = true;
        }

        var rarities = new[] { BabyRarities.Common, BabyRarities.Rare, BabyRarities.SuperRare, BabyRarities.Legendary };
        var accumulatedRarities = rarities
            .Select((x, i) => x + rarities.Take(i).Sum());

        var rarity = accumulatedRarities.First(x => x >= roll);
        return rarity;
    }

    private Baby InstantiateBaby(Type type)
    {
        var baby = (Baby)GetType().GetMethod(nameof(InstantiateBabyGeneric)).MakeGenericMethod(type).Invoke(this, null);
        return baby;
    }

    public TBaby InstantiateBabyGeneric<TBaby>() where TBaby : Baby, new()
    {
        return new TBaby();
    }
}