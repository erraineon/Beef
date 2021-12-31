using System.Collections;

namespace Beef.Core.Chats.Discord;

public interface IDiscordOptions
{
    string Token { get; }
    IList<ulong> TestGuildIds { get; }
}