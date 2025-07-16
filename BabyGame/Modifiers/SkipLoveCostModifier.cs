using System.ComponentModel;
using BabyGame.Data;
using BabyGame.Events;

namespace BabyGame.Modifiers;

[DisplayName("Buff: Free Love")]
[Description("Love without paying Chu")]
public class SkipLoveCostModifier : Modifier, IEventHandler<IChuCostMultiplierOnLove, double>
{
    public IEnumerable<double> Handle(BabyEventArgs<IChuCostMultiplierOnLove, double> eventArgs)
    {
        yield return 0;
    }
}