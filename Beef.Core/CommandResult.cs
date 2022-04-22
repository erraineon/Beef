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
        if (result is string x && string.IsNullOrWhiteSpace(x) || result == null)
            result = "👌";

        return new CommandResult(
            null,
            string.Empty,
            result
        );
    }

    public static RuntimeResult Fail(Exception ex)
    {
        return new CommandResult(
            new InteractionCommandError(),
            ex.Message,
            ex.Message
        );
    }
}