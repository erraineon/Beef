namespace Beef.Core.Data;

public class CronTrigger : TimeTrigger
{
    public required string CronSchedule { get; set; }

    public override void Advance(DateTime utcNow)
    {
        throw new NotImplementedException();
    }
}