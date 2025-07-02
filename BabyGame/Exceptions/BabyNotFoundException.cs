namespace BabyGame.Exceptions;

public class BabyNotFoundException(string babyName) : Exception
{
    public string BabyName { get; } = babyName;
}