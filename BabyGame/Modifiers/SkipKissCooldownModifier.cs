using BabyGame.Data;
using BabyGame.Events;
using System.ComponentModel;

namespace BabyGame.Modifiers;

[Description("Kiss again")]
[DisplayName("Buff: Another Kiss")]
public class SkipKissCooldownModifier : Modifier, IKissCooldownMultiplierOnKiss
{
    public IEnumerable<double> Handle(BabyEventArgs<IKissCooldownMultiplierOnKiss, double> eventArgs)
    {
        yield return 0;
    }
}