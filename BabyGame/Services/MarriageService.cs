using BabyGame.Data;
using BabyGame.Exceptions;

namespace BabyGame.Services;

public class MarriageService(IBabyGameRepository babyGameRepository, ITimeProvider timeProvider)
{
    public async Task MarryAsync(ulong user1Id, ulong user2Id)
    {
        EnsureNoSelfMarriage(user1Id, user2Id);
        await EnsureNotAlreadyMarriedAsync(user1Id);
        await EnsureNotAlreadyMarriedAsync(user2Id);
        await babyGameRepository.AddMarriageAsync(
            new Marriage
            {
                User1Id = user1Id,
                User2Id = user2Id,
                MarriedAt = timeProvider.Now,
                LastKissedAt = timeProvider.Now
            }
        );
    }

    private async Task EnsureNotAlreadyMarriedAsync(ulong userId)
    {
        if (await babyGameRepository.GetIsMarriedAsync(userId))
            throw new AlreadyMarriedException();
    }

    private static void EnsureNoSelfMarriage(ulong user1Id, ulong user2Id)
    {
        if (user1Id == user2Id)
            throw new SelfMarriageException(user1Id);
    }
}