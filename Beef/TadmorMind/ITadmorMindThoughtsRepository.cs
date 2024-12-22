namespace Beef.TadmorMind;

public interface ITadmorMindThoughtsRepository
{
    Task<string> ReceiveAsync();
    Task<int> GetCountAsync();
    Task AddRangeAsync(List<string> entries);
}