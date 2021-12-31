using Discord;

namespace Beef.Core.Chats.Discord;

public interface IDiscordChatClient : IChatClient
{
    Task LoginAsync(TokenType tokenType, string token, bool validateToken = true);
    event Func<Task> Ready;
}