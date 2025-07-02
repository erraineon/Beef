namespace BabyGame.Exceptions;

internal class NotEnoughChuException(decimal chu) : Exception
{
    public decimal Chu { get; } = chu;
}