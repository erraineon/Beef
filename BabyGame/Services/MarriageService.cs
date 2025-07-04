using BabyGame.Data;
using BabyGame.Exceptions;
using BabyGame.Modifiers;
using Humanizer;

namespace BabyGame.Services;

public class MarriageService(
    IBabyGameConfiguration configuration,
    IBabyGameRepository babyGameRepository,
    IModifierService modifierService,
    IBabyGameLogger babyGameLogger,
    IRandomProvider randomProvider,
    ITimeProvider timeProvider)
{
    public async Task<Marriage> MarryAsync(Player spouse1, Player spouse2)
    {
        EnsureNoSelfMarriage(spouse1, spouse2);
        await EnsureNotAlreadyMarriedAsync(spouse1);
        await EnsureNotAlreadyMarriedAsync(spouse2);
        var now = timeProvider.Now;
        var marriage = new Marriage
        {
            Spouse1 = spouse1,
            Spouse2 = spouse2,
            MarriedAt = now,
            Seed = randomProvider.NextInt(null, int.MinValue, int.MaxValue)
        };
        // TODO: probably want to separate both these in their own services
        marriage.Affinity = randomProvider.NextInt(marriage, 1, configuration.MaxInitialAffinity);
        marriage.Chu = randomProvider.NextInt(marriage, 1, 31);

        await babyGameRepository.CreateMarriageAsync(marriage);
        await modifierService.AddModifierAsync(marriage, new SkipLoveCostModifier { ChargesLeft = 1 }, false);

        // TODO: stretch between 1 and 1000
        var compatibility = marriage.Affinity switch
        {
            <= 1 => "Awful!",
            <= 5 => "Bad...",
            <= 10 => "Good!",
            <= 15 => "Fantastic!!",
            _ => "Unexpected."
        };
        babyGameLogger.Log(
            $"{spouse1.DisplayName} and {spouse2.DisplayName} are now married. " +
            $"Their compatibility is... {compatibility} It's as if they've known " +
            $"each other for {"day".ToQuantity(marriage.Affinity, ShowQuantityAs.Words)}."
        );

        return marriage;
    }

    private async Task EnsureNotAlreadyMarriedAsync(Player player)
    {
        if (await babyGameRepository.GetIsMarriedAsync(player))
            throw new AlreadyMarriedException(player);
    }

    private static void EnsureNoSelfMarriage(Player spouse1, Player spouse2)
    {
        if (spouse1.Id == spouse2.Id)
            throw new NoSelfMarriage(spouse1);
    }
}