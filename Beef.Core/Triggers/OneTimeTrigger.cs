namespace Beef.Core.Triggers;

public record OneTimeTrigger(TriggerContext Context, DateTimeOffset FireAt) : TimeTrigger(Context, FireAt);