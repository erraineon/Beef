namespace BabyGame.Exceptions;

public class NotMarriedException(ulong userId) : Exception
{
    public ulong UserId { get; } = userId;
}