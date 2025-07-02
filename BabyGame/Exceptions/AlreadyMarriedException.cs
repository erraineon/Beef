using BabyGame.Data;

namespace BabyGame.Exceptions;

public class AlreadyMarriedException(Spouse spouse) : Exception
{
    public Spouse Spouse { get; } = spouse;
}