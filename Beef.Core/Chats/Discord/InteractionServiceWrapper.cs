using Discord.Interactions;
using Discord.Rest;
using Discord.WebSocket;

namespace Beef.Core.Chats.Discord;

public class InteractionServiceWrapper : InteractionService, IDiscordInteractionService
{
    public InteractionServiceWrapper(DiscordSocketClient discord, InteractionServiceConfig? config = null) : base(discord, config)
    {
    }

    public InteractionServiceWrapper(DiscordShardedClient discord, InteractionServiceConfig? config = null) : base(discord, config)
    {
    }

    public InteractionServiceWrapper(BaseSocketClient discord, InteractionServiceConfig? config = null) : base(discord, config)
    {
    }

    public InteractionServiceWrapper(DiscordRestClient discord, InteractionServiceConfig? config = null) : base(discord, config)
    {
    }
}