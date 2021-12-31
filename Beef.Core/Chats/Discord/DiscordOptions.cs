namespace Beef.Core.Chats.Discord;

public record DiscordOptions(string Token, IList<ulong> TestGuildIds) : IDiscordOptions;
