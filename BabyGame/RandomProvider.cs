using BabyGame.Data;

namespace BabyGame;

public class RandomProvider : IRandomProvider
{
    public double NextDouble(Marriage marriage)
    {
        // Probably should be retrieved from a dictionary with a scoped lifetime instead of modifying the seed directly
        var random = new Random(marriage.Seed);
        marriage.Seed = random.Next();
        return random.NextDouble();
    }
    public T NextItem<T>(Marriage marriage, T[] values)
    {
        // Probably should be retrieved from a dictionary with a scoped lifetime instead of modifying the seed directly
        var random = new Random(marriage.Seed);
        marriage.Seed = random.Next();
        return random.GetItems(values, 1)[0];
    }
}