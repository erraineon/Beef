using Discord;

namespace Beef.Core.Interactions;

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

    public ulong Id => throw new NotSupportedException();
    public string Name { get; }
    public IReadOnlyCollection<IApplicationCommandInteractionDataOption> Options { get; }
}