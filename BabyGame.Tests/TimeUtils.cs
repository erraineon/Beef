namespace BabyGame.Tests;

public static class TimeUtils
{
    public static DateTimeOffset MarriageDay { get; } = new(2025, 6, 1, 12, 30, 0, TimeSpan.Zero);
    public static DateTimeOffset AfterMarriage { get; } = new(2025, 7, 2, 14, 15, 0, TimeSpan.Zero);
}