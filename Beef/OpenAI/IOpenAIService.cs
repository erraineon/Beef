namespace Beef.OpenAI;

public interface IOpenAIService{
    Task<string> GenerateCompletionAsync(string prompt);
    Task<string> GenerateChatCompletionAsync(string prompt);
}