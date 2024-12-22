using Discord;

namespace Beef.Core.Interactions;

public interface IMessageContentPreprocessor
{
    string? GetProcessedInputOrNull(IUserMessage value);
}