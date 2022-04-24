using Discord.Interactions;

namespace Beef.Core;

public class SuccessResult : RuntimeResult
{
    private SuccessResult(InteractionCommandError? error, string errorReason, object? result) : base(error, errorReason)
    {
        Result = result;
    }

    public object? Result { get; }

    public static RuntimeResult Ok(object? result = default)
    {
        return new SuccessResult(
            null,
            string.Empty,
            result
        );
    }
}