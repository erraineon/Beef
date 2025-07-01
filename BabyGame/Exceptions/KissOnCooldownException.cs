namespace BabyGame.Exceptions;

internal class KissOnCooldownException(TimeSpan cooldown) : Exception
{
    public TimeSpan Cooldown { get; } = cooldown;
}