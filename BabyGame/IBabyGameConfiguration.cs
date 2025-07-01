namespace BabyGame;

public interface IBabyGameConfiguration
{
    TimeSpan MinimumKissCooldown { get; set; }
    TimeSpan BaseKissCooldown { get; set; }
    double MaxAffinityKissCooldownMultiplier { get; set; }
    double AffinityKissCooldownMultiplierRate { get; set; }
    double BaseLoveCost { get; set; }
    int MaxBabies { get; set; }
}