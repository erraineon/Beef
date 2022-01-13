using Discord;

namespace Beef.Core.Chats.Interactions.Execution;

public record BotInteractionContext(
    IDiscordClient Client,
    IGuild Guild,
    IMessageChannel Channel,
    IUser User,
    IDiscordInteraction Interaction
) : IInteractionContext;