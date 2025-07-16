using System.ComponentModel;
using BabyGame.Data;
using BabyGame.Events;

namespace BabyGame.Modifiers;

[Description("Kiss daily for more bonuses")]
[DisplayName("Love Affinity")]
public class AffinityModifier : Modifier, IEventHandler<IChuMultiplierOnKiss, double>
{
    public IEnumerable<double> Handle(BabyEventArgs<IChuMultiplierOnKiss, double> eventArgs)
    {
        yield return Effectiveness / 100.0;
    }
}