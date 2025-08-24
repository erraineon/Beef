using BabyGame.Data;

namespace BabyGame;

public interface IKissService
{
    Task KissAsync(Player player);
}