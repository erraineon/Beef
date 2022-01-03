namespace Beef.Core.Triggers;

public record MessageTrigger(TriggerContext Context, string Regex) : Trigger(Context);