namespace Beef.TadmorMind;

public class TadmorMindOptions
{
    public required string ServiceAddress { get; set; }
    public double SpeakUpProbability { get; set; } = 0.01;
}