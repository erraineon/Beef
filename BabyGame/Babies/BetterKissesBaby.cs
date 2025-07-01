using System.ComponentModel;
using BabyGame.Data;

namespace BabyGame.Babies;

[Description("Earn more Chu")]
public class BetterKissesBaby : Baby, IChuAdder
{
    // TODO: for random babies, store a current seed on the marriage so it yields consistent results for each kiss
    public double GetChu(ICollection<Baby> babyGroup)
    {
        // https://www.desmos.com/calculator/ctrhg7idl5
        var l = Level;
        var x = babyGroup.Count;
        var result = MathF.Pow(2, l) + MathF.PI * MathF.Log2(x);
        return result;
    }
}