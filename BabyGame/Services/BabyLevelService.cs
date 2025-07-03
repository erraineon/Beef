using BabyGame.Data;
using BabyGame.Extensions;

namespace BabyGame.Services;

public class BabyLevelService(
    IBabyGameRepository babyGameRepository,
    IBabyGameLogger logger
) : IBabyLevelService
{
    public async Task GainExperienceAsync(Baby baby, double chu)
    {
        EnsureBabyBelowMaxRank(baby);
        var currentLevel = baby.Level;
        baby.TotalExperience += chu;
        baby.Level = RecalculateLevel(baby);
        await babyGameRepository.SaveChangesAsync();
        if (currentLevel != baby.Level) logger.Log($"{baby.Name} has become rank {baby.GetRank()}!");
        logger.Log($"{baby.Name} needs {GetExperiencePointsUntilNextLevel(baby)} EXP to rank up.");
    }

    private static void EnsureBabyBelowMaxRank(Baby baby)
    {
        if (baby.IsMaxRank())
            throw new InvalidOperationException("A max rank baby can't gain EXP.");
    }

    public int RecalculateLevel(Baby baby)
    {
        //https://www.desmos.com/calculator/0rc80gouni
        var x = baby.TotalExperience;
        return (int)Math.Floor(!baby.IsStarRank() ? Math.Cbrt(x) : 10 + (int)Math.Log2(x / 1000));
    }

    public double GetExperiencePointsUntilNextLevel(Baby baby)
    {
        var x = baby.Level;
        var levelExperience = !baby.IsStarRank() ? Math.Pow(x, 3) : Math.Pow(2, x - 10) * 1000;
        return baby.TotalExperience - levelExperience;
    }
}