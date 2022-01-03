namespace Beef.Core;

public interface ICurrentTimeProvider
{
    DateTimeOffset Now { get; }
}