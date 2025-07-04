using System.Numerics;
using BabyGame.Data;

namespace BabyGame.Events;

public class BabyEventArgs<TEvent, TResult> where TResult : IAdditionOperators<TResult, TResult, TResult>
{
    public Marriage Marriage { get; set; }
    public IEventAggregate<TResult> ProgressSoFar { get; set; }
}