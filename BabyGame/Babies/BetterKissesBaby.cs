using BabyGame.Data;
using System.ComponentModel;
using BabyGame.Events;

namespace BabyGame.Babies;

[Description("Earn more Chu")]
public class BetterKissesBaby : Baby, IChuOnKiss
{
    public IEnumerable<double> Handle(BabyEventArgs<IChuOnKiss, double> eventArgs)
    {
        // https://www.desmos.com/calculator/wbzc7cqhlf
        var x = Level;
        var phi = 0.5 * (1 + Math.Sqrt(5)) / 2;
        var result = Math.Pow(x, phi) + Math.Pow(Math.PI, x - 10);
        yield return result;
    }
}