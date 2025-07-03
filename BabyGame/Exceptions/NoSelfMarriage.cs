using BabyGame.Data;

namespace BabyGame.Exceptions;

public class NoSelfMarriage(Player player) : Exception
{
    public Player Player { get; } = player;
}