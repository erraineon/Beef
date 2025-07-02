using BabyGame.Data;

namespace BabyGame.Exceptions;

public class SelfMarriageException(Spouse spouse) : Exception
{
    public Spouse Spouse { get; } = spouse;
}