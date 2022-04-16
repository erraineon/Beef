namespace Beef.Core.Triggers;

public record IntervalTrigger(TriggerContext Context, DateTimeOffset FireAt, TimeSpan Interval) : TimeTrigger(
    Context,
    FireAt
);