using Discord;

namespace Beef.Core.Telegram
{
    public class TelegramAttachment : IAttachment
    {
        public ulong Id => throw new System.NotImplementedException();
        public string Filename { get; init; }
        public string Url => throw new System.NotImplementedException();
        public string ProxyUrl => throw new System.NotImplementedException();
        public int Size => throw new System.NotImplementedException();
        public int? Height => throw new System.NotImplementedException();
        public int? Width => throw new System.NotImplementedException();
        public bool Ephemeral => false;
        public string Description => throw new System.NotImplementedException();
        public string ContentType => throw new System.NotImplementedException();
    }
}