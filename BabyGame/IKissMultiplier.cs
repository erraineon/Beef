using BabyGame.Data;

namespace BabyGame;

internal interface IKissMultiplier
{
    double GetKissMultiplier(ICollection<Baby> babyGroup);
}