using Discord;

namespace Beef.Core.Triggers;

public record BotInteractionContext(
    IDiscordClient Client,
    IGuild Guild,
    IMessageChannel Channel,
    IUser User,
    IDiscordInteraction Interaction
) : IInteractionContext;