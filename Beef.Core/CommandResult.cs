using Discord.Interactions;

namespace Beef.Core;

public class CommandResult : RuntimeResult
{
    private CommandResult(InteractionCommandError? error, string errorReason, object result) : base(error, errorReason)
    {
        Result = result;
    }

    public object Result { get; }

    public static RuntimeResult Ok(object? result = default)
    {
        result ??= "👌";
        return new CommandResult(
            null,
            string.Empty,
            result
        );
    }
}