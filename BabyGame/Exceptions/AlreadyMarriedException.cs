using BabyGame.Data;

namespace BabyGame.Exceptions;

public class AlreadyMarriedException(Marriage existingMarriages) : Exception
{
    public Marriage ExistingMarriages { get; } = existingMarriages;
}