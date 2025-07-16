using BabyGame.Data;
using BabyGame.Events;
using BabyGame.Exceptions;

namespace BabyGame.Services;

public class MarriageService(
    IBabyGameRepository babyGameRepository,
    IRandomProvider randomProvider,
    ITimeProvider timeProvider,
    IEventDispatcher eventDispatcher)
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
        marriage.Chu = randomProvider.NextInt(marriage, 1, 31);
        eventDispatcher.Aggregate<IMarriageComplete, int>(marriage);
        await babyGameRepository.CreateMarriageAsync(marriage);
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