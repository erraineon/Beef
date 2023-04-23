using Discord.Interactions;

namespace Beef.Core;

public class SuccessResult : RuntimeResult
{
    public SuccessResult(object? result = default) : base(null, string.Empty)
    {
        Result = result;
    }

    public object? Result { get; }
}