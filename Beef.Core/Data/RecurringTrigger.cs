namespace Beef.Core.Data;

public class RecurringTrigger : TimeTrigger
{
    public TimeSpan Interval { get; set; }

    public override void Advance(DateTime utcNow)
    {
        TriggerAtUtc = utcNow + Interval;
    }
}