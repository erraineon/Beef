using System.Numerics;
using BabyGame.Data;
using BabyGame.Events;

namespace BabyGame;

public interface IEventDispatcher
{
    EventAggregate<TResult> Aggregate<TEventHandler, TResult>(Marriage marriage)
        where TEventHandler : IEventHandler<TEventHandler, TResult>
        where TResult : IAdditionOperators<TResult, TResult, TResult>;
}