namespace Beef.Gpt3;

public interface IGpt3Client
{
    Task<List<string>> GenerateEntriesAsync();
    Task<string> GenerateCompletionAsync(string prompt);
}