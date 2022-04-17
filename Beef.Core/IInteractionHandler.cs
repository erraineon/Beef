using Discord;

namespace Beef.Core;

public interface IInteractionHandler
{
    void HandleInteractionContext(IInteractionContext context);
}