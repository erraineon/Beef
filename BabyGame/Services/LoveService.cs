using BabyGame.Data;
using BabyGame.Events;
using BabyGame.Exceptions;
using BabyGame.Models;
using Humanizer;
using Microsoft.Extensions.Options;

namespace BabyGame.Services;

public class LoveService(
    IOptionsSnapshot<BabyGameOptions> configuration,
    IBabyGameRepository babyGameRepository,
    IBabyGachaService babyGachaService,
    IEventDispatcher eventDispatcher,
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
        var baby = babyGachaService.CreateBaby(marriage, babyName);

        var nextCost = GetLoveCost(marriage, today, out _);
        var tomorrow = timeProvider.Today.AddDays(1).Humanize(true, dateToCompareAgainst: timeProvider.Now.UtcDateTime);
        logger.Log(
            nextCost switch
            {
                > 0 => $"The next baby will cost {nextCost} until {tomorrow}",
                0 => "The next baby will be free.",
                _ => "The next baby will be free, and then some!"
            }
        );
        await babyGameRepository.SaveMarriageAsync(marriage);
    }

    private decimal GetLoveCost(Marriage marriage, DateTime today, out int timesLovedToday)
    {
        timesLovedToday = marriage.LastLovedOn != null && marriage.LastLovedOn.Value.Date <= today
            ? marriage.TimesLovedOnLastDate
            : 0;
        var loveCostMultiplier = 1 - eventDispatcher.Aggregate<IChuCostMultiplierOnLove, double>(marriage).Sum();
        var loveCost = Math.Pow(configuration.Value.BaseLoveCost, ++timesLovedToday) * loveCostMultiplier;
        return (decimal)loveCost;
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