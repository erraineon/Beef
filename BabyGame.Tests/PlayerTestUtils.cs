using BabyGame.Data;

namespace BabyGame.Tests;

public static class PlayerTestUtils
{
    public static Player GetBob()
    {
        return new Player { DisplayName = "Bob", Id = 67890, UserName = "@bob" };
    }

    public static Player GetAlice()
    {
        return new Player { DisplayName = "Alice", Id = 12345, UserName = "@alice" };
    }
}