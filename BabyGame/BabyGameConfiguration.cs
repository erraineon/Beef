namespace BabyGame;

public class BabyGameConfiguration : IBabyGameConfiguration
{
    public TimeSpan MinimumKissCooldown { get; set; } = TimeSpan.FromMinutes(1);
    public TimeSpan BaseKissCooldown { get; set; } = TimeSpan.FromMinutes(10);
    public double MaxAffinityKissCooldownMultiplier { get; set; } = 0.66;
    public double AffinityKissCooldownMultiplierRate { get; set; } = 4.5;
    public double BaseLoveCost { get; set; } = 10;
    public int MaxBabies { get; set; } = 20;
}