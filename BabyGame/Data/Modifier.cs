using BabyGame.Services;

namespace BabyGame.Data;

public class Modifier
{
    public Guid Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? EndsAt { get; set; }
    public int? ChargesLeft { get; set; }
    public int TimesUsed { get; set; }
    public Marriage Marriage { get; set; }
}