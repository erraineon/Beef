using System.ComponentModel;
using System.Reflection;
using BabyGame.Data;
using BabyGame.Exceptions;
using BabyGame.Modifiers;
using Humanizer;
using Microsoft.Extensions.Options;

namespace BabyGame.Services;

public class LoveService(
    IOptionsSnapshot<IBabyGameConfiguration> configuration,
    IBabyGameRepository babyGameRepository,
    IBabyGachaService babyGachaService,
    IModifierService modifierService,
    IBabyGameLogger logger,
    ITimeProvider timeProvider)
{
    public async Task LoveAsync(Player player, string babyName)
    {
        var marriage = await babyGameRepository.GetMarriageAsync(player);
        EnsureEnoughSpace(marriage);
        var today = timeProvider.Today;
        var loveCost = GetLoveCost(marriage, today, out var timesLovedToday);
        EnsureEnoughChu(marriage, loveCost);

        marriage.Chu -= loveCost;
        marriage.LastLovedOn = today;
        marriage.TimesLovedOnLastDate = timesLovedToday;
        await modifierService.TryUseModifierAsync<SkipLoveCostBuff>(marriage);
        var baby = await babyGachaService.CreateBabyAsync(marriage, babyName);

        var babyTypeName = baby.GetType().GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ??
                           baby.GetType().Name.Humanize();
        logger.Log($"Mr. Stork delivered {baby.Name}, the {babyTypeName} ({baby.GetRank()})");
        var nextCost = GetLoveCost(marriage, today, out _);
        var tomorrow = timeProvider.Today.AddDays(1);
        logger.Log($"The next baby will cost {nextCost} until {tomorrow.Humanize()}");
    }

    private decimal GetLoveCost(Marriage marriage, DateTime today, out int timesLovedToday)
    {
        timesLovedToday = marriage.LastLovedOn != null && marriage.LastLovedOn.Value.Date <= today
            ? marriage.TimesLovedOnLastDate
            : 0;
        var loveCost = (decimal)(modifierService.GetModifierOrNull<SkipLoveCostBuff>(marriage) != null
            ? 0
            : Math.Pow(configuration.Value.BaseLoveCost, ++timesLovedToday));
        return loveCost;
    }

    private void EnsureEnoughSpace(Marriage marriage)
    {
        if (marriage.Babies.Count >= configuration.Value.MaxBabies)
            throw new TooManyBabiesException();
    }

    private void EnsureEnoughChu(Marriage marriage, decimal loveCost)
    {
        var chu = marriage.Chu;
        if (chu < loveCost)
            throw new NotEnoughChuException(chu);
    }
}