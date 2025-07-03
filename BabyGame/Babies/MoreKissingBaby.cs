using System.ComponentModel;
using BabyGame.Data;
using BabyGame.Events;

namespace BabyGame.Babies;

[Description("Kiss more often")]
public class MoreKissingBaby : Baby, IKissCooldownMultiplierOnKiss
{
    public IEnumerable<double> Handle(BabyEventArgs<IKissCooldownMultiplierOnKiss, double> eventArgs)
    {
        // https://www.desmos.com/calculator/oti77sak1f
        // Unsure if I even want this
        throw new NotImplementedException();
    }
}