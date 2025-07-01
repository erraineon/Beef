namespace BabyGame.Exceptions;

internal class NotEnoughChuToLoveException(decimal chu) : Exception
{
    public decimal Chu { get; } = chu;
}