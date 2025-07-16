namespace BabyGame.Data;

public class Marriage
{
    public Guid Id { get; init; }
    public Player Spouse1 { get; init; }
    public Player Spouse2 { get; init; }
    public List<Modifier> Modifiers { get; set; } = new();
    public DateTimeOffset MarriedAt { get; init; }
    public decimal Chu { get; set; }
    public List<Baby> Babies { get; init; } = new();
    public int Kisses { get; set; }
    public DateTimeOffset? LastKissedAt { get; set; }
    public double Pity { get; set; }
    public int Seed { get; set; }
    public DateTime? LastLovedOn { get; set; }
    public int TimesLovedOnLastDate { get; set; }
}