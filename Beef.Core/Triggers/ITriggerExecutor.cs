namespace Beef.Core.Triggers;

public interface ITriggerExecutor
{
    Task ExecuteAsync(Trigger trigger);
}