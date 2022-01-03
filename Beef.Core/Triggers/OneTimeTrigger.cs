namespace Beef.Core.Triggers;

public record OneTimeTrigger(TriggerContext Context, DateTime FireAt, TimeSpan Interval) : TimeTrigger(Context, FireAt);