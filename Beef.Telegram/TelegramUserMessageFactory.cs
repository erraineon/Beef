using Discord;
using Telegram.Bot.Types;

namespace Beef.Telegram
{
    public class TelegramUserMessageFactory : ITelegramUserMessageFactory
    {
        private readonly IUserMessageCache _userMessageCache;

        public TelegramUserMessageFactory(IUserMessageCache userMessageCache)
        {
            _userMessageCache = userMessageCache;
        }

        public IUserMessage Create(Message apiMessage, ITextChannel channel, IGuildUser author)
        {
            IEnumerable<IAttachment> GetAttachments()
            {
                if (apiMessage.Photo is { } photos)
                    yield return CreateAttachment(photos.Last().FileId);
            }

            var referencedMessage = apiMessage.ReplyToMessage is { MessageId: var messageId }
                ? _userMessageCache.GetCachedUserMessages(channel.GuildId)
                    .FirstOrDefault(um => um.Id == (ulong)messageId)
                : null;

            var message = new TelegramUserMessage
            {
                ApiMessage = apiMessage,
                Attachments = GetAttachments().ToList(),
                Channel = channel,
                Author = author,
                ReferencedMessage = referencedMessage,
            };
            return message;
        }

        private TelegramAttachment CreateAttachment(string fileId)
        {
            var attachment = new TelegramAttachment
            {
                Filename = fileId
            };
            return attachment;
        }
    }
}