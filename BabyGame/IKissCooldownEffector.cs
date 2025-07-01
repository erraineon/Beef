using BabyGame.Data;

namespace BabyGame;

public interface IKissCooldownEffector
{
    double GetCooldownMultiplierDeduction(ICollection<Baby> babyGroup);
}