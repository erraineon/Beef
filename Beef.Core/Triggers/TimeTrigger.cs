namespace Beef.Core.Triggers;

public abstract record TimeTrigger(TriggerContext Context, DateTime FireAt) : Trigger(Context);