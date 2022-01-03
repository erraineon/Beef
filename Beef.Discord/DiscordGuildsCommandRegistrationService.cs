using Beef.Core.Chats.Interactions.Registration;
using Discord;
using Discord.Interactions;

namespace Beef.Discord;

public class DiscordGuildsCommandRegistrationService : ICommandRegistrationService
{
    private readonly IDiscordChatClient _discordChatClient;
    private readonly IDiscordCommandRegistrar _discordCommandRegistrar;

    public DiscordGuildsCommandRegistrationService(
        IDiscordChatClient discordChatClient,
        IDiscordCommandRegistrar discordCommandRegistrar
    )
    {
        _discordChatClient = discordChatClient;
        _discordCommandRegistrar = discordCommandRegistrar;
    }

    public async Task RegisterCommandsAsync()
    {
        var botOwner = (await _discordChatClient.GetApplicationInfoAsync()).Owner;
        foreach (var guild in await _discordChatClient.GetGuildsAsync())
        {
            await _discordCommandRegistrar.RegisterCommandsToGuildAsync(guild.Id);
            await GiveOwnerPermissionsAsync(guild, botOwner);
        }
    }

    private async Task GiveOwnerPermissionsAsync(IGuild guild, IUser botOwner)
    {
        await Task.WhenAll(
            _discordCommandRegistrar.SlashCommands
                .Where(x => !x.DefaultPermission && x.Attributes.OfType<RequireOwnerAttribute>().Any())
                .Select(
                    ownerCommand => _discordCommandRegistrar.ModifySlashCommandPermissionsAsync(
                        ownerCommand,
                        guild,
                        new ApplicationCommandPermission(botOwner, true)
                    )
                )
        );
    }
}