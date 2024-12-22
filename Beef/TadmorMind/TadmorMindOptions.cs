namespace Beef.TadmorMind;

public class TadmorMindOptions
{
    public required string ServiceAddress { get; set; }
    public double SpeakUpProbability { get; set; } = 0.01;
    public List<ulong> MonitoredGuildIds { get; set; } = new();
}