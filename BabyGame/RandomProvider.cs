using BabyGame.Data;

namespace BabyGame;

public class RandomProvider : IRandomProvider
{
    public double NextDouble(Marriage marriage)
    {
        var random = new Random(marriage.Seed);
        marriage.Seed = random.Next();
        return random.NextDouble();
    }
    public int NextInt(Marriage marriage, int min, int max)
    {
        var random = new Random(marriage.Seed);
        marriage.Seed = random.Next();
        return random.Next(min, max);
    }
    public T NextItem<T>(Marriage marriage, T[] values)
    {
        var random = new Random(marriage.Seed);
        marriage.Seed = random.Next();
        return random.GetItems(values, 1)[0];
    }
}