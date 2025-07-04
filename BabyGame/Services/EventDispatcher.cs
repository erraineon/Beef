using System.Numerics;
using BabyGame.Data;
using BabyGame.Events;

namespace BabyGame.Services;

public class EventDispatcher(IModifierService modifierService) : IEventDispatcher
{
    public IEventAggregate<TResult> Aggregate<TEventHandler, TResult>(Marriage marriage)
        where TEventHandler : IEventHandler<TEventHandler, TResult>
        where TResult : IAdditionOperators<TResult, TResult, TResult>
    {
        var aggregate = new EventAggregate<TResult>();
        var eventArgs = new BabyEventArgs<TEventHandler, TResult>
        {
            Marriage = marriage,
            ProgressSoFar = aggregate
        };
        var contributors = marriage.Babies.Where(x => x.GraduationDate == null)
            .OfType<object>()
            .Concat(modifierService.GetActiveModifiers<Modifier>(marriage))
            .OfType<TEventHandler>();

        foreach (var eventHandler in contributors)
        {
            var contributions = eventHandler.Handle(eventArgs).ToList();
            if (contributions.Any())
            {
                aggregate.Contributions[eventHandler] = contributions;
                if (eventHandler is Modifier modifier)
                    modifierService.UseModifier(modifier);
            }
        }

        return aggregate;
    }
}