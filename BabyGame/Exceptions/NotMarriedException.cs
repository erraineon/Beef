using BabyGame.Data;

namespace BabyGame.Exceptions;

public class NotMarriedException(Spouse spouse) : Exception
{
    public Spouse Spouse { get; } = spouse;
}