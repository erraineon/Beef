using Discord;
using Telegram.Bot.Types;

namespace Beef.Telegram
{
    public interface ITelegramUserMessageFactory
    {
        IUserMessage Create(Message apiMessage, ITextChannel channel, IGuildUser author);
    }
}