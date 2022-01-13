using NCrontab;

namespace Beef.Core.Triggers;

public class TimeTriggerFactory : ITimeTriggerFactory
{
    private readonly ICurrentTimeProvider _currentTimeProvider;

    public TimeTriggerFactory(ICurrentTimeProvider currentTimeProvider)
    {
        _currentTimeProvider = currentTimeProvider;
    }

    public TimeTrigger? Advance(TimeTrigger timeTrigger)
    {
        // TODO: change this to Create
        var now = _currentTimeProvider.Now;
        return timeTrigger switch
        {
            IntervalTrigger intervalTrigger => intervalTrigger with {FireAt = now + intervalTrigger.Interval},
            OneTimeTrigger => default,
            ReminderTrigger => default,
            ScheduleTrigger scheduleTrigger => scheduleTrigger with
            {
                FireAt = new DateTimeOffset(
                    CrontabSchedule.Parse(scheduleTrigger.Schedule).GetNextOccurrence(now.UtcDateTime)
                )
            },
            _ => throw new ArgumentOutOfRangeException(nameof(timeTrigger))
        };
    }
}