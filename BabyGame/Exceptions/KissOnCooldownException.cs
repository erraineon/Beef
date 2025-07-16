namespace BabyGame.Exceptions;

public class KissOnCooldownException(TimeSpan cooldown) : Exception
{
    public TimeSpan Cooldown { get; } = cooldown;
}