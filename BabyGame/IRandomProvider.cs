using BabyGame.Data;

namespace BabyGame;

public interface IRandomProvider
{
    double NextDouble(Marriage marriage);
    T NextItem<T>(Marriage marriage, T[] values);
}