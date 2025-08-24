using BabyGame.Data;

namespace BabyGame;

public interface IMarriageService
{
    Task<Marriage> MarryAsync(Player spouse1, Player spouse2);
}