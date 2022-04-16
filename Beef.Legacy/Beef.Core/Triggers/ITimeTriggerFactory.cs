namespace Beef.Core.Triggers;

public interface ITimeTriggerFactory
{
    TimeTrigger? Advance(TimeTrigger timeTrigger);
}