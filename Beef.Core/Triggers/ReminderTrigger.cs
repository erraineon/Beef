namespace Beef.Core.Triggers;

public record ReminderTrigger(TriggerContext Context, DateTimeOffset FireAt, string Reminder) : TimeTrigger(Context, FireAt);