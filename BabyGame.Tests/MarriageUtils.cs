using BabyGame.Data;

namespace BabyGame.Tests;

public static class MarriageUtils
{
    public static Marriage GetMarriage()
    {
        return new Marriage
        {
            Spouse1 = PlayerUtils.GetAlice(),
            Spouse2 = PlayerUtils.GetBob(),
            MarriedAt = TimeUtils.MarriageDay,
        };
    }
}