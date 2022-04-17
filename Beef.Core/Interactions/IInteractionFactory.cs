using Discord;

namespace Beef.Core.Interactions;

public interface IInteractionFactory
{
    IDiscordInteraction CreateInteraction(IUser user, IMessageChannel channel, string command);
}