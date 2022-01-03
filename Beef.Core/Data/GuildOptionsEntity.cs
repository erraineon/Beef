namespace Beef.Core.Data;

public class GuildOptionsEntity
{
    public ulong Id { get; set; }
    public ChatType ChatType { get; set; }
    public GuildOptions Value { get; set; } = new();
}