using BabyGame.Data;

namespace BabyGame.Exceptions;

public class NotMarriedException(Player player) : Exception
{
    public Player Player { get; } = player;
}