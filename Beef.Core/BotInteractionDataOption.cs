using Discord;

namespace Beef.Core;

public class BotInteractionDataOption : IApplicationCommandInteractionDataOption
{
    public BotInteractionDataOption(
        string name,
        string? value,
        ApplicationCommandOptionType type,
        IReadOnlyCollection<IApplicationCommandInteractionDataOption>? options
    )
    {
        Name = name;
        Value = value;
        Type = type;
        Options = options;
    }

    public string Name { get; }
    public object? Value { get; }
    public ApplicationCommandOptionType Type { get; }
    public IReadOnlyCollection<IApplicationCommandInteractionDataOption>? Options { get; }
}