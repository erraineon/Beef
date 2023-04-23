using System.Reflection;
using Beef.Core.Discord;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Beef.Core.Interactions;

public class InteractionRegistrar : IHostedService
{
    private readonly DiscordSocketClient _discordClient;

    private readonly InteractionService _interactionService;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DiscordClientLauncher> _logger;
    public InteractionRegistrar(DiscordSocketClient discordClient, InteractionService interactionService, IServiceProvider serviceProvider, ILogger<DiscordClientLauncher> logger)
    {
        _discordClient = discordClient;
        _interactionService = interactionService;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), _serviceProvider);
        await _interactionService.AddModulesAsync(GetType().Assembly, _serviceProvider);

        _discordClient.GuildAvailable += RegisterCommandsToGuildAsync;
        _discordClient.JoinedGuild += RegisterCommandsToGuildAsync;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
    private async Task RegisterCommandsToGuildAsync(SocketGuild guild)
    {
        try
        {
            var botOwner = (await _discordClient.GetApplicationInfoAsync()).Owner;
            await _interactionService.RegisterCommandsToGuildAsync(guild.Id);

            // TODO: remove
            //var giveOwnerPermissions = _interactionService.SlashCommands
            //    .Where(x => !x.DefaultPermission && x.Preconditions.OfType<RequireOwnerAttribute>().Any())
            //    .Select(
            //        ownerCommand => _interactionService.ModifySlashCommandPermissionsAsync(
            //            ownerCommand,
            //            guild,
            //            new ApplicationCommandPermission(botOwner, true)
            //        )
            //    );

            //var giveRolePermissions = _interactionService.SlashCommands
            //    .Where(x => !x.DefaultPermission)
            //    .Select(
            //        ownerCommand =>
            //        {
            //            var roleAttribute = ownerCommand.Preconditions.OfType<RequireRoleAttribute>().FirstOrDefault();
            //            var role = roleAttribute == null
            //                ? null
            //                : roleAttribute.RoleId.HasValue
            //                    ? guild.GetRole(roleAttribute.RoleId.Value)
            //                    : guild.Roles.FirstOrDefault(x => x.Name == roleAttribute.RoleName);
            //            return (ownerCommand, role);
            //        }
            //    )
            //    .Where(t => t.role != null)
            //    .Select(
            //        t =>
            //        {
            //            var (ownerCommand, role) = t;
            //            return _interactionService.ModifySlashCommandPermissionsAsync(
            //                ownerCommand,
            //                guild,
            //                new ApplicationCommandPermission(role, true)
            //            );
            //        }
            //    );
            //await Task.WhenAll(giveOwnerPermissions.Concat(giveRolePermissions));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while registering commands against a guild.");
        }
    }
}