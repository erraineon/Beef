using Discord;

namespace Beef.Core;

public class BotInteractionData : IApplicationCommandInteractionData
{
    public BotInteractionData(
        string name,
        IReadOnlyCollection<IApplicationCommandInteractionDataOption> options
    )
    {
        Name = name;
        Options = options;
    }

    public ulong Id => throw new NotImplementedException();
    public string Name { get; }
    public IReadOnlyCollection<IApplicationCommandInteractionDataOption> Options { get; }
}