using Beef.Core.Chats;
using Discord;
using Discord.WebSocket;

namespace Beef.Discord;

public interface IDiscordChatClient : IChatClient
{
    Task LoginAsync(TokenType tokenType, string token, bool validateToken = true);
    event Func<Task> Ready;
    event Func<SocketInteraction, Task> InteractionCreated;
}