namespace Beef.Core.Data;

public abstract class Trigger
{
    public int Id { get; set; }
    public ChatType ChatType { get; set; }
    public ulong GuildPermissionsRawValue { get; set; }
    public ulong GuildId { get; set; }
    public ulong ChannelId { get; set; }
    public ulong UserId { get; set; }
    public required string CommandToRun { get; set; }
}