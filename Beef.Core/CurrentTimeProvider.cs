namespace Beef.Core;

public class CurrentTimeProvider : ICurrentTimeProvider
{
    public DateTimeOffset Now => DateTimeOffset.Now;
}