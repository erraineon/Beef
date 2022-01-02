using Discord;
using Discord.Interactions;

namespace Beef.Core.Chats.Interactions.Execution;

public interface IInteractionExecutor
{
    Task<IResult> ExecuteInteractionAsync(IInteractionContext interactionContext);
}