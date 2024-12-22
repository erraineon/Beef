using System.Collections.Concurrent;

namespace Beef.Core.Telegram;

public class FixedSizedQueue<T>(int size) : ConcurrentQueue<T>
{
    private readonly object _syncObject = new();

    public new void Enqueue(T obj)
    {
        base.Enqueue(obj);
        lock (_syncObject)
        {
            while (Count > size) TryDequeue(out _);
        }
    }
}