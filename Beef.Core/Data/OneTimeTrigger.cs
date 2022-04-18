namespace Beef.Core.Data;

public class OneTimeTrigger : TimeTrigger
{
    public override void Advance(DateTime utcNow)
    {
        TriggerAtUtc = utcNow;
    }
}