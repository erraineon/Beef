using System.Numerics;
using BabyGame.Data;
using BabyGame.Events;
using Microsoft.Extensions.DependencyInjection;

namespace BabyGame.Services;

public class EventDispatcher(IModifierService modifierService, IServiceProvider serviceProvider) : IEventDispatcher
{
    public IEventAggregate<TResult> Aggregate<TEvent, TResult>(Marriage marriage)
        where TEvent : IEvent<TResult>
        where TResult : IAdditionOperators<TResult, TResult, TResult>
    {
        var aggregate = new EventAggregate<TResult>();
        var eventArgs = new BabyEventArgs<TEvent, TResult>
        {
            Marriage = marriage,
            ProgressSoFar = aggregate
        };
        using var serviceScope = serviceProvider.CreateScope();
        var contributors = marriage.Babies.Where(x => x.GraduationDate == null)
            .OfType<object>()
            .Concat(modifierService.GetActiveModifiers<Modifier>(marriage))
            .OfType<IEventHandler<TEvent, TResult>>()
            .Concat(serviceScope.ServiceProvider.GetServices<IEventHandler<TEvent, TResult>>());

        foreach (var eventHandler in contributors)
        {
            var entityType = eventHandler.GetType();
            var interceptorResults = (IEnumerable<TResult>)GetType().GetMethod(nameof(AggregateFromProxy))
                .MakeGenericMethod(entityType, typeof(TEvent), typeof(TResult))
                .Invoke(this, [eventHandler, eventArgs, serviceScope]);

            var contributions = eventHandler.Handle(eventArgs).Concat(interceptorResults).ToList();
            if (contributions.Any())
            {
                aggregate.Contributions[eventHandler] = contributions;
                if (eventHandler is Modifier modifier)
                    modifierService.UseModifier(modifier);
            }
        }

        return aggregate;
    }

    public IEnumerable<TResult> AggregateFromProxy<TProxy, TEvent, TResult>(TProxy proxy,
        BabyEventArgs<TEvent, TResult> eventArgs, IServiceScope serviceScope)
        where TEvent : IEvent<TResult>
        where TResult : IAdditionOperators<TResult, TResult, TResult>
    {
        var interceptorResults = serviceScope.ServiceProvider
            .GetServices<IEventHandler<TProxy, TEvent, TResult>>()
            .SelectMany(x => x.Handle(proxy, eventArgs));
        return interceptorResults;
    }
}