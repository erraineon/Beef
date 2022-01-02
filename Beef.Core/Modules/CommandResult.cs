using Discord.Interactions;

namespace Beef.Core.Modules;

public class CommandResult : RuntimeResult, ICommandResult
{
    private CommandResult(InteractionCommandError? error, string errorReason, object result) : base(error, errorReason)
    {
        Result = result;
    }

    public static RuntimeResult Ok(object result)
    {
        return new CommandResult(
            null,
            string.Empty,
            result
        );
    }

    public object Result { get; }
}