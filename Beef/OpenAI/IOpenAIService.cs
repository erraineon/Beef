namespace Beef.OpenAi;

public interface IOpenAiService{
    Task<string> GenerateChatCompletionAsync(string prompt);
}