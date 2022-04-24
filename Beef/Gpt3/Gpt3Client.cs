using Flurl.Http;
using Microsoft.Extensions.Options;

namespace Beef.Gpt3;

public class Gpt3Client : IGpt3Client
{
    private const int MaxTokensToGenerate = 256;
    private readonly IOptions<Gpt3Options> _gpt3Gpt3Options;

    public Gpt3Client(IOptions<Gpt3Options> gpt3Gpt3Options)
    {
        _gpt3Gpt3Options = gpt3Gpt3Options;
    }

    public async Task<List<string>> GenerateEntriesAsync()
    {
        var text = await CompleteAsync(
            new
            {
                prompt = string.Empty,
                model = _gpt3Gpt3Options.Value.ModelName,
                max_tokens = MaxTokensToGenerate
            }
        );
        var entryLimiters = new[] { "<|endoftext|>", "END" };
        var entriesStartIndex = Math.Max(0, entryLimiters.Min(e => text.IndexOf(e, StringComparison.Ordinal)));
        var random = new Random();
        var entries = text[entriesStartIndex..]
            .Split(entryLimiters, StringSplitOptions.RemoveEmptyEntries)[..^1]
            .Select(RemoveWhiteSpace)
            .Where(e => !string.IsNullOrWhiteSpace(e))
            .Distinct()
            .OrderBy(_ => random.Next())
            .ToList();
        return entries;
    }

    public async Task<string> GenerateCompletionAsync(string prompt)
    {
        var completion = await CompleteAsync(
            new
            {
                prompt,
                model = _gpt3Gpt3Options.Value.ModelName,
                max_tokens = MaxTokensToGenerate,
                temperature = 0.75f,
                stop = " END"
            }
        );
        string result;
        if (prompt.EndsWith("?"))
        {
            var separator = completion.FirstOrDefault() is var c && (char.IsLetterOrDigit(c) || c == '"')
                ? " "
                : string.Empty;
            result = $"{prompt}{separator}{completion}";
        }
        else result = completion;

        return result;
    }

    private async Task<string> CompleteAsync(object requestData)
    {
        var result = await "https://api.openai.com/v1/completions"
            .WithOAuthBearerToken(_gpt3Gpt3Options.Value.ApiKey)
            .PostJsonAsync(requestData);
        var response = await result.GetJsonAsync();
        var text = response.choices[0].text.ToString() as string ?? throw new Exception("no data was generated");
        return RemoveWhiteSpace(text);
    }

    private static string RemoveWhiteSpace(string text)
    {
        return text.Replace("\n\n", "\n").Trim('\r', '\n', ' ');
    }
}