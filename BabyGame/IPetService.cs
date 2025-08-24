using BabyGame.Data;

namespace BabyGame;

public interface IPetService
{
    Task PetAsync(Player player, int chu, string babyName);
}