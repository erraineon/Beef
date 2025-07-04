using System.Numerics;
using BabyGame.Data;
using BabyGame.Events;

namespace BabyGame.Tests;

public class TestEventDispatcher(Dictionary<Type, object> results) : IEventDispatcher
{
    private Dictionary<Type, object> Results { get; } = results;

    public IEventAggregate<TResult> Aggregate<TEventHandler, TResult>(Marriage marriage) where TEventHandler : IEventHandler<TEventHandler, TResult> where TResult : IAdditionOperators<TResult, TResult, TResult>
    {
        var aggregate = new EventAggregate<TResult>();
        if (Results.TryGetValue(typeof(TEventHandler), out var result))
            aggregate.Contributions["Test"] = new List<TResult> { (TResult)result };

        return aggregate;
    }
}