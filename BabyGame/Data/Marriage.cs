namespace BabyGame.Data;

public class Marriage
{
    public Guid Id { get; init; }
    public required ulong User1Id { get; init; }
    public required ulong User2Id { get; init; }
    public required DateTimeOffset MarriedAt { get; init; }
    public decimal Chu { get; set; }
    public List<Baby> Babies { get; init; } = new();
    public int Affinity { get; set; }
    public int Kisses { get; set; }
    public required DateTimeOffset LastKissedAt { get; set; }
    public bool SkipNextCooldown { get; set; }
    public bool SkipNextLoveCost { get; set; }
    public double Pity { get; set; }
    public int Seed { get; set; }
    public DateTime LastLovedOn { get; set; }
    public int TimesLovedOnLastDate { get; set; }
}