using Discord;

namespace Beef.Core;

public interface IInteractionFactory
{
    IDiscordInteraction CreateInteraction(IUser user, IMessageChannel channel, string command);
}