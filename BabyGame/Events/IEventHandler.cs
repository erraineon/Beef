using System.Numerics;

namespace BabyGame.Events;

public interface IEventHandler<TEventHandler, TResult> where TResult : IAdditionOperators<TResult, TResult, TResult>
{
    public IEnumerable<TResult> Handle(BabyEventArgs<TEventHandler, TResult> eventArgs);
}