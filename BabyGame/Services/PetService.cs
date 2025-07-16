using BabyGame.Data;
using BabyGame.Exceptions;
using BabyGame.Extensions;
using Microsoft.Extensions.Options;

namespace BabyGame.Services;

public class PetService(
    IOptionsSnapshot<IBabyGameConfiguration> configuration,
    IBabyGameRepository babyGameRepository,
    IBabyGachaService babyGachaService,
    IBabyLevelService babyLevelService,
    IBabyGameLogger logger,
    ITimeProvider timeProvider)
{
    public async Task PetAsync(Player player, int chu, string babyName)
    {
        EnsureChuValid(chu);
        var marriage = await babyGameRepository.GetMarriageAsync(player);
        var baby = marriage.GetBaby(babyName);
        EnsureEnoughChu(marriage, chu);
        // TODO: experience modifiers
        babyLevelService.GainExperience(baby, chu);
    }

    private void EnsureChuValid(int chu)
    {
        if (chu <= 0) throw new InvalidOperationException("Chu can't be negative.");
    }

    private void EnsureEnoughChu(Marriage marriage, int petCost)
    {
        if (marriage.Chu < petCost)
            throw new NotEnoughChuException(marriage.Chu);
    }
}