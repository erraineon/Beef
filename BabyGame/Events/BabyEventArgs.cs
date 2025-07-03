using System.Numerics;
using BabyGame.Data;

namespace BabyGame.Events;

public class BabyEventArgs<TEvent, T> where T : IAdditionOperators<T, T, T>
{
    public Marriage Marriage { get; set; }
    public EventAggregate<T> ProgressSoFar { get; set; }
}