using System.ComponentModel;
using BabyGame.Data;
using BabyGame.Events;

namespace BabyGame.Modifiers;

[Description("Kiss again")]
[DisplayName("Buff: Another Kiss")]
public class SkipKissCooldownModifier : Modifier, IEventHandler<IKissCooldownMultiplierOnKiss, double>
{
    public IEnumerable<double> Handle(BabyEventArgs<IKissCooldownMultiplierOnKiss, double> eventArgs)
    {
        yield return 0;
    }
}