using Discord;

namespace Beef.Telegram
{
    public interface IUserMessageCache
    {
        IEnumerable<IUserMessage> GetCachedUserMessages(ulong guildId);
        void AddUserMessage(IUserMessage userMessage, ulong guildId);
    }
}