using System.ComponentModel;
using BabyGame.Data;
using MathNet.Symbolics;

namespace BabyGame.Babies;

[Description("Kiss more often")]
public class MoreKissingBaby : Baby, IKissCooldownEffector
{
    public double GetCooldownMultiplierDeduction(ICollection<Baby> babyGroup)
    {
        // https://www.desmos.com/calculator/oti77sak1f
        // Unsure if I even want this
        throw new NotImplementedException();
    }
}