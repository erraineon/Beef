using System.Collections.Concurrent;

namespace Beef.Core.Telegram;

public class FixedSizedQueue<T> : ConcurrentQueue<T>
{
    private readonly int _size;
    private readonly object _syncObject = new();

    public FixedSizedQueue(int size)
    {
        _size = size;
    }

    public new void Enqueue(T obj)
    {
        base.Enqueue(obj);
        lock (_syncObject)
        {
            while (Count > _size) TryDequeue(out _);
        }
    }
}