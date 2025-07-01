namespace BabyGame;

public interface ITimeProvider
{
    DateTimeOffset Now { get; }
    DateTime Today { get; }
}