namespace Beef.Core.Triggers;

public record IntervalTrigger(TriggerContext Context, DateTime FireAt, TimeSpan Interval) : TimeTrigger(Context, FireAt);