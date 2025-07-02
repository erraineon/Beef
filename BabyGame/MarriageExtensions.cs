using BabyGame.Data;
using BabyGame.Exceptions;

namespace BabyGame;

public static class MarriageExtensions
{
    public static Baby GetBaby(this Marriage marriage, string babyName)
    {
        var baby = marriage.Babies.FirstOrDefault(x => string.Equals(
                x.Name,
                babyName,
                StringComparison.OrdinalIgnoreCase
            )
        );
        if (baby == null)
            throw new BabyNotFoundException(babyName);

        return baby;
    }
}