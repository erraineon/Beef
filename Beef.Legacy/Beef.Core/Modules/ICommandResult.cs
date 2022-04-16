using Discord.Interactions;

namespace Beef.Core.Modules;

public interface ICommandResult : IResult
{
    object Result { get; }
}