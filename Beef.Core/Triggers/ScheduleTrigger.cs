namespace Beef.Core.Triggers;

public record ScheduleTrigger(TriggerContext Context, DateTime FireAt, string Schedule) : TimeTrigger(Context, FireAt);