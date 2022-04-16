namespace Beef.Core.Triggers;

public abstract record TimeTrigger(TriggerContext Context, DateTimeOffset FireAt) : Trigger(Context);