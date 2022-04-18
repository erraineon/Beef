namespace Beef.Core.Data;

public abstract class TimeTrigger : Trigger
{
    public DateTime? TriggerAtUtc { get; set; }

    public abstract void Advance(DateTime utcNow);
}