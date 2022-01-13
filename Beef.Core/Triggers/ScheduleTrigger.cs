namespace Beef.Core.Triggers;

public record ScheduleTrigger(TriggerContext Context, DateTimeOffset FireAt, string Schedule) : TimeTrigger(
    Context,
    FireAt
);