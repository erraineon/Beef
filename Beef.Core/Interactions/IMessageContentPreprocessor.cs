namespace Beef.Core.Interactions;

public interface IMessageContentPreprocessor
{
    string? GetProcessedInputOrNull(string value);
}