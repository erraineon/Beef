namespace BabyGame;

public class TimeProvider : ITimeProvider
{
    public DateTimeOffset Now => DateTimeOffset.Now;
    public DateTime Today => DateTime.Today;
}