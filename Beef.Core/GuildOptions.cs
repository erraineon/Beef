using Beef.Core.Triggers;

namespace Beef.Core;

public class GuildOptions
{
    public List<Trigger> Triggers { get; init; } = new();
}