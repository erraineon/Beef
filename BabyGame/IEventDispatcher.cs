using System.Numerics;
using BabyGame.Data;
using BabyGame.Events;

namespace BabyGame;

public interface IEventDispatcher
{
    IEventAggregate<TResult> Aggregate<TEvent, TResult>(Marriage marriage)
        where TEvent : IEvent<TResult>
        where TResult : IAdditionOperators<TResult, TResult, TResult>;
}