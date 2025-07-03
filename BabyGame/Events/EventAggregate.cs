using System.Numerics;
using BabyGame.Data;
using BabyGame.Extensions;
using BabyGame.Services;
using Humanizer;

namespace BabyGame.Events;

public class EventAggregate<TResult> where TResult : IAdditionOperators<TResult, TResult, TResult>
{
    public Dictionary<object, ICollection<TResult>> Contributions { get; set; } = new();

    public TResult Sum()
    {
        return Contributions.SelectMany(x => x.Value)
            .Aggregate((acc, curr) => acc + curr);
    }

    public IEnumerable<(string, TResult)> ContributionsByType => Contributions
        .GroupBy(x => x.Key.GetType())
        .Select(x => (
                x.Select(y => GetContributorName(y.Key)).Humanize(),
                x.SelectMany(y => y.Value).Aggregate((acc, curr) => acc + curr)
            )
        );

    private static string GetContributorName(object y)
    {
        return y switch
        {
            Baby baby => baby.Name,
            Modifier modifier => modifier.GetDisplayName(),
            _ => "A mysterious entity"
        };
    }

    public EventAggregate<TResult> LogByType(IBabyGameLogger logger, string format)
    {
        return LogByType(logger, (x, y) => string.Format(format, x, y));
    }
    public EventAggregate<TResult> LogByType(IBabyGameLogger logger, Func<string, TResult, string> format)
    {
        foreach (var (contributorNames, contribution) in ContributionsByType)
            logger.Log(format(contributorNames, contribution));
        return this;
    }
}