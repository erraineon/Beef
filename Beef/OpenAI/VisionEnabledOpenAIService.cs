using OpenAI.Extensions;
using OpenAI.Interfaces;
using OpenAI.Managers;
using OpenAI.ObjectModels.ResponseModels;

namespace Beef.OpenAi;

public interface IVisionEnabledOpenAIService : IOpenAIService
{
    Task<ChatCompletionCreateResponse> CreateVisionCompletion(VisionChatCompletionCreateRequest chatCompletionCreate, string? modelId = null);
}

public class VisionEnabledOpenAIService : OpenAIService, IVisionEnabledOpenAIService
{
    private readonly HttpClient _httpClient;

    public VisionEnabledOpenAIService(OpenAI.OpenAiOptions settings, HttpClient httpClient) : base(settings, httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ChatCompletionCreateResponse> CreateVisionCompletion(VisionChatCompletionCreateRequest chatCompletionCreate, string? modelId = null)
    {
        chatCompletionCreate.Model = modelId;
        return await _httpClient.PostAndReadAsAsync<ChatCompletionCreateResponse>($"v1/chat/completions", chatCompletionCreate);
    }
}