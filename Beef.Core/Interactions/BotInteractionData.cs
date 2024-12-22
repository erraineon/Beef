using Discord;

namespace Beef.Core.Interactions;

public class BotInteractionData(
    string name,
    IReadOnlyCollection<IApplicationCommandInteractionDataOption> options)
    : IApplicationCommandInteractionData
{
    public ulong Id => throw new NotSupportedException();
    public string Name { get; } = name;
    public IReadOnlyCollection<IApplicationCommandInteractionDataOption> Options { get; } = options;
}