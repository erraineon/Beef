using BabyGame.Data;

namespace BabyGame;

public class BabyGraduationService(IBabyGameLogger logger)
{
    public async Task GraduateAsync(Marriage marriage, string babyName)
    {
        var baby = marriage.GetBaby(babyName);
        // https://www.desmos.com/calculator/zdk88surzp
        var x = baby.TotalExperience;
        var z = 1 / (1 + Math.Exp(-0.02 * (x - 1000)));
        var reward = x * z + (x / 10 + Math.Pow(x / 50, 2) * (1 - z));
        // TODO: if fully ranked up, trigger bonuses
        // TODO: chance of not leaving based off Affinity
        logger.Log($"{baby.Name} has graduated, leaving {reward} Chu behind. Goodbye, {baby.Name}!");
    }
}