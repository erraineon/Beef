namespace BabyGame.Data;

public class Marriage
{
    public Guid Id { get; init; }
    public Spouse Spouse1 { get; init; }
    public Spouse Spouse2 { get; init; } 
    public required DateTimeOffset MarriedAt { get; init; }
    public decimal Chu { get; set; }
    public List<Baby> Babies { get; init; } = new();
    public int Affinity { get; set; }
    public int Kisses { get; set; }
    public DateTimeOffset? LastKissedAt { get; set; }
    public bool SkipNextCooldown { get; set; }
    public bool SkipNextLoveCost { get; set; }
    public double Pity { get; set; }
    public int Seed { get; set; }
    public DateTime? LastLovedOn { get; set; }
    public int TimesLovedOnLastDate { get; set; }
}