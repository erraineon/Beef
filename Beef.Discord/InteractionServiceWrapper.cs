using Beef.Core.Chats.Interactions.Execution;
using Beef.Core.Data;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace Beef.Discord;


public class InteractionServiceWrapper : InteractionService, IDiscordCommandRegistrar, IInteractionService
{
    public InteractionServiceWrapper(IDiscordChatClient discord) : base(
        (DiscordSocketClient)discord,
        new InteractionServiceConfig { DefaultRunMode = RunMode.Sync }
    )
    {
    }

    public ChatType ChatType => ChatType.Discord;
}