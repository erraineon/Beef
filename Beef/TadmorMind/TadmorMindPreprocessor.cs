using Beef.Core.Interactions;
using Microsoft.Extensions.Options;

namespace Beef.TadmorMind;

public class TadmorMindPreprocessor(IOptionsSnapshot<TadmorMindOptions> tadmorMindOptions) : IMessageContentPreprocessor
{
    public string? GetProcessedInputOrNull(string value)
    {
        return !value.StartsWith('.') &&
               (new[] { "tadmor", "taddy" }.Any(x => value.Contains(x, StringComparison.InvariantCultureIgnoreCase)) ||
                Random.Shared.NextDouble() <= tadmorMindOptions.Value.SpeakUpProbability)
            ? ".gen"
            : null;
    }
}