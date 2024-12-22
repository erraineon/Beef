namespace Beef.TadmorMind;

public interface ITadmorMindClient
{
    Task<List<string>> GenerateEntriesAsync();
}