using BabyGame.Data;

namespace BabyGame;

internal interface IChuAdder
{
    double GetChu(ICollection<Baby> babyGroup);
}