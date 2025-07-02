using BabyGame.Data;
using BabyGame.Exceptions;

namespace BabyGame.Services;

public class MarriageService(IBabyGameRepository babyGameRepository, IBabyGameLogger babyGameLogger, IRandomProvider randomProvider, ITimeProvider timeProvider)
{
    public async Task MarryAsync(Spouse spouse1, Spouse spouse2)
    {
        EnsureNoSelfMarriage(spouse1, spouse2);
        await babyGameRepository.CreateOrUpdateSpouse(spouse1);
        await babyGameRepository.CreateOrUpdateSpouse(spouse2);
        await EnsureNotAlreadyMarriedAsync(spouse1);
        await EnsureNotAlreadyMarriedAsync(spouse2);
        var marriage = new Marriage
        {
            Spouse1 = new Spouse(),
            Spouse2 = new Spouse(),
            MarriedAt = timeProvider.Now,
            Seed = Random.Shared.Next()
        };
        marriage.Affinity = randomProvider.NextInt(marriage, 1, 15);
        await babyGameRepository.SaveMarriageAsync(marriage);

        var compatibility = marriage.Affinity switch
        {
            <= 1 => "Awful!",
            <= 5 => "Bad...",
            <= 10 => "Good!",
            <= 15 => "Fantastic!!",
            _ => "Unexpected."
        };
        babyGameLogger.Log($"{spouse1.DisplayName} and {spouse2.DisplayName} are now married. Their compatibility is... {compatibility} It's as if they've known each other for {marriage.Affinity} day(s).");
    }

    private async Task EnsureNotAlreadyMarriedAsync(Spouse spouse)
    {
        if (await babyGameRepository.GetIsMarriedAsync(spouse))
            throw new AlreadyMarriedException(spouse);
    }

    private static void EnsureNoSelfMarriage(Spouse spouse1, Spouse spouse2)
    {
        if (spouse1.Id == spouse2.Id)
            throw new SelfMarriageException(spouse1);
    }
}