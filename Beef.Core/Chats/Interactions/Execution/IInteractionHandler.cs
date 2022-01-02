using Discord;

namespace Beef.Core.Chats.Interactions.Execution;

public interface IInteractionHandler
{
    void HandleInteractionNoAwait(IInteractionContext context);
}