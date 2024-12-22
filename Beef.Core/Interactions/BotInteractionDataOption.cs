using Discord;

namespace Beef.Core.Interactions;

public class BotInteractionDataOption(
    string name,
    string? value,
    ApplicationCommandOptionType type,
    IReadOnlyCollection<IApplicationCommandInteractionDataOption>? options)
    : IApplicationCommandInteractionDataOption
{
    public string Name { get; } = name;
    public object? Value { get; } = value;
    public ApplicationCommandOptionType Type { get; } = type;
    public IReadOnlyCollection<IApplicationCommandInteractionDataOption>? Options { get; } = options;
}