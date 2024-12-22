using Discord.Interactions;

namespace Beef.Core;

public class SuccessResult(object? result = default) : RuntimeResult(null, string.Empty)
{
    public object? Result { get; } = result;
}