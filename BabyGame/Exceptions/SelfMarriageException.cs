namespace BabyGame.Exceptions;

public class SelfMarriageException(ulong userId) : Exception
{
    public ulong UserId { get; } = userId;
}