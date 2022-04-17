using Discord;

namespace Beef.Core.Interactions;

public interface IInteractionHandler
{
    void HandleInteractionContext(IInteractionContext context);
}