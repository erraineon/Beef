using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using OpenAI.Interfaces;
using OpenAI.ObjectModels.SharedModels;

namespace Beef.OpenAi;

public class VisionChatCompletionCreateRequest : IOpenAiModels.ITemperature, IOpenAiModels.IModel
{
    [JsonPropertyName("messages")]
    public IList<VisionChatMessage> Messages { get; set; }

    [JsonIgnore]
    public string? Stop { get; set; }

    /// <summary>
    ///     Up to 4 sequences where the API will stop generating further tokens. The returned text will not contain the stop
    ///     sequence.
    /// </summary>
    [JsonIgnore]
    public IList<string>? StopAsList { get; set; }

    [JsonPropertyName("stop")]
    public IList<string>? StopCalculated
    {
        get
        {
            if (Stop != null && StopAsList != null)
            {
                throw new ValidationException("Stop and StopAsList can not be assigned at the same time. One of them is should be null.");
            }

            if (Stop != null)
            {
                return new List<string> { Stop };
            }

            return StopAsList;
        }
    }

    [JsonPropertyName("max_tokens")]
    public int? MaxTokens { get; set; }

    [JsonPropertyName("model")]
    public string? Model { get; set; }

    [JsonPropertyName("temperature")]
    public float? Temperature { get; set; }
}