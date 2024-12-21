using Discord;

namespace Beef.Core.Telegram;

public class TelegramAttachment : IAttachment
{
    public ulong Id => throw new NotImplementedException();
    public string Filename { get; init; }
    public string Url => throw new NotImplementedException();
    public string ProxyUrl => throw new NotImplementedException();
    public int Size => throw new NotImplementedException();
    public int? Height => throw new NotImplementedException();
    public int? Width => throw new NotImplementedException();
    public bool Ephemeral => false;
    public string Description => throw new NotImplementedException();
    public string ContentType => throw new NotImplementedException();
    public double? Duration { get; }
    public string Waveform { get; }
    public byte[] WaveformBytes { get; }
    public AttachmentFlags Flags { get; }
    public IReadOnlyCollection<IUser> ClipParticipants { get; }
    public string Title { get; }
    public DateTimeOffset? ClipCreatedAt { get; }
    public DateTimeOffset CreatedAt { get; }
}