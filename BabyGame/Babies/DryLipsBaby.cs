using System.ComponentModel;
using BabyGame.Attributes;
using BabyGame.Data;
using BabyGame.Events;
using BabyGame.Models;

namespace BabyGame.Babies;

[Rarity(BabyRarities.Legendary)]
[Description("Earn no Chu when you kiss.")]
public class DryLipsBaby : Baby, IEventHandler<IChuMultiplierOnKiss, double>
{
    public IEnumerable<double> Handle(BabyEventArgs<IChuMultiplierOnKiss, double> eventArgs)
    {
        yield return 0;
    }
}