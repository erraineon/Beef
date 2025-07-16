using System.Numerics;

namespace BabyGame.Events;

public interface IEvent<TResult> : IEvent
{
}

public interface IEventHandler<TEvent, TResult> where TEvent : IEvent<TResult>
    where TResult : IAdditionOperators<TResult, TResult, TResult>
{
    IEnumerable<TResult> Handle(BabyEventArgs<TEvent, TResult> eventArgs);
}

public interface IEventHandler<in TProxy, TEvent, TResult>
    where TEvent : IEvent<TResult>
    where TResult : IAdditionOperators<TResult, TResult, TResult>
{
    public IEnumerable<TResult> Handle(TProxy proxy, BabyEventArgs<TEvent, TResult> eventArgs);
}