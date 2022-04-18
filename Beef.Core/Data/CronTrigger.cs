namespace Beef.Core.Data;

public class CronTrigger : TimeTrigger
{
    public string CronSchedule { get; set; } = null!;

    public override void Advance(DateTime utcNow)
    {
        throw new NotImplementedException();
    }
}