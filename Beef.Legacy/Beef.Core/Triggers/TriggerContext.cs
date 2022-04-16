using Beef.Core.Data;

namespace Beef.Core.Triggers;

public record TriggerContext(ChatType ChatType, ulong GuildId, ulong ChannelId, ulong UserId, string Command);