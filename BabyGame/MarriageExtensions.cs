using System.Numerics;
using BabyGame.Babies;
using BabyGame.Data;
using BabyGame.Events;
using BabyGame.Exceptions;

namespace BabyGame;

public static class MarriageExtensions
{
    public static Baby GetBaby(this Marriage marriage, string babyName)
    {
        var baby = marriage.Babies.FirstOrDefault(x => string.Equals(
                x.Name,
                babyName,
                StringComparison.OrdinalIgnoreCase
            )
        );
        if (baby == null)
            throw new BabyNotFoundException(babyName);

        return baby;
    }

    public static EventAggregate<TResult> Aggregate<TEventHandler, TResult>(this Marriage marriage)
        where TEventHandler : IEventHandler<TEventHandler, TResult>
        where TResult : IAdditionOperators<TResult, TResult, TResult>
    {
        var aggregate = new EventAggregate<TResult>();
        var eventArgs = new BabyEventArgs<TEventHandler, TResult>
        {
            Marriage = marriage,
            ProgressSoFar = aggregate
        };
        foreach (var baby in marriage.Babies.Where(x => x.GraduationDate == null))
            if (baby is TEventHandler handler)
            {
                var contributions = handler.Handle(eventArgs).ToList();
                if (contributions.Any()) aggregate.Contributions[baby] = contributions;
            }

        return aggregate;
    }
}