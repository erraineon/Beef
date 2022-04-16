using Discord;

namespace Beef.Core.Chats.Interactions.Execution;

public interface IInteractionFactory
{
    IDiscordInteraction CreateInteraction(IUser user, IMessageChannel channel, string text);
}