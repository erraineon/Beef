using BabyGame.Data;
using BabyGame.Services;
using System.ComponentModel;
using BabyGame.Events;

namespace BabyGame.Modifiers;

[DisplayName("Buff: Free Love")]
[Description("Love without paying Chu")]
public class SkipLoveCostModifier : Modifier, IChuCostMultiplierOnLove
{
    public IEnumerable<double> Handle(BabyEventArgs<IChuCostMultiplierOnLove, double> eventArgs)
    {
        yield return 0;
    }
}