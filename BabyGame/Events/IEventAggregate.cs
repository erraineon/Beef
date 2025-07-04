using System.Numerics;

namespace BabyGame.Events;

public interface IEventAggregate<TResult> where TResult : IAdditionOperators<TResult, TResult, TResult>
{
    Dictionary<object, ICollection<TResult>> Contributions { get; set; }
    IEnumerable<(string, TResult)> ContributionsByType { get; }
    TResult Sum();
    IEventAggregate<TResult> LogByType(IBabyGameLogger logger, string format);
    IEventAggregate<TResult> LogByType(IBabyGameLogger logger, Func<string, TResult, string> format);
}