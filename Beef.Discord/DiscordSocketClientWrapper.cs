using Beef.Core.Data;
using Discord.WebSocket;

namespace Beef.Discord;

public class DiscordSocketClientWrapper : DiscordSocketClient, IDiscordChatClient
{
    public ChatType ChatType => ChatType.Discord;
}