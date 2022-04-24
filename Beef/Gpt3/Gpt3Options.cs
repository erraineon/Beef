namespace Beef.Gpt3;

public class Gpt3Options
{
    public string ApiKey { get; set; } = null!;
    public string ModelName { get; set; } = null!;
    public double Temperature { get; set; } = 1f;
    public int MaxAttempts { get; set; } = 3;
}