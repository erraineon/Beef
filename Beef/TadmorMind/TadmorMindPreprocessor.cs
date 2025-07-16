using Beef.Core.Interactions;
using Discord;
using Microsoft.Extensions.Options;

namespace Beef.TadmorMind;

public class TadmorMindPreprocessor(IOptionsSnapshot<TadmorMindOptions> tadmorMindOptions) : IMessageContentPreprocessor
{
    public string? GetProcessedInputOrNull(IUserMessage value)
    {
        return value.Channel is IGuildChannel guildChannel &&
               tadmorMindOptions.Value.MonitoredGuildIds.Contains(guildChannel.GuildId) &&
               !value.Content.StartsWith('.') &&
               (new[] { "tadmor", "taddy" }.Any(x => value.Content.Contains(
                        x,
                        StringComparison.InvariantCultureIgnoreCase
                    )
                ) ||
                Random.Shared.NextDouble() <= tadmorMindOptions.Value.SpeakUpProbability)
            ? ".gen"
            : null;
    }
}