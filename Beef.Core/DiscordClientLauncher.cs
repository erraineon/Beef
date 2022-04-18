using System.Reflection;
using Beef.Core.Interactions;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Beef.Core;

public class DiscordClientLauncher : IHostedService
{
    private readonly DiscordSocketClient _discordClient;
    private readonly IOptions<DiscordOptions> _discordOptions;
    private readonly IInteractionHandler _interactionHandler;
    private readonly InteractionService _interactionService;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DiscordClientLauncher> _logger;

    public DiscordClientLauncher(
        IOptions<DiscordOptions> discordOptions,
        DiscordSocketClient discordClient,
        IServiceProvider serviceProvider,
        InteractionService interactionService,
        IInteractionHandler interactionHandler,
        ILogger<DiscordClientLauncher> logger
    )
    {
        _discordOptions = discordOptions;
        _discordClient = discordClient;
        _serviceProvider = serviceProvider;
        _interactionService = interactionService;
        _interactionHandler = interactionHandler;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _discordClient.InteractionCreated += OnInteractionCreatedAsync;
        _discordClient.GuildAvailable += RegisterCommandsToGuildAsync;
        _discordClient.JoinedGuild += RegisterCommandsToGuildAsync;

        await _interactionService.AddModulesAsync(GetType().Assembly, _serviceProvider);

        var discordReady = new TaskCompletionSource();

        Task OnReady()
        {
            _discordClient.Ready -= OnReady;
            discordReady.SetResult();
            return Task.CompletedTask;
        }

        _discordClient.Ready += OnReady;

        await _discordClient.LoginAsync(TokenType.Bot, _discordOptions.Value.Token);
        await _discordClient.StartAsync();
        await discordReady.Task;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _discordClient.StopAsync();
    }

    private Task OnInteractionCreatedAsync(SocketInteraction interaction)
    {
        var context = new SocketInteractionContext(
            _discordClient,
            interaction
        );
        _interactionHandler.HandleInteractionContext(context);
        return Task.CompletedTask;
    }

    private async Task RegisterCommandsToGuildAsync(SocketGuild guild)
    {
        try
        {
            var botOwner = (await _discordClient.GetApplicationInfoAsync()).Owner;
            await _interactionService.RegisterCommandsToGuildAsync(guild.Id);

            var giveOwnerPermissions = _interactionService.SlashCommands
                .Where(x => !x.DefaultPermission && x.Preconditions.OfType<RequireOwnerAttribute>().Any())
                .Select(
                    ownerCommand => _interactionService.ModifySlashCommandPermissionsAsync(
                        ownerCommand,
                        guild,
                        new ApplicationCommandPermission(botOwner, true)
                    )
                );

            var giveRolePermissions = _interactionService.SlashCommands
                .Where(x => !x.DefaultPermission)
                .Select(
                    ownerCommand =>
                    {
                        var roleAttribute = ownerCommand.Preconditions.OfType<RequireRoleAttribute>().FirstOrDefault();
                        var role = roleAttribute == null
                            ? null
                            : roleAttribute.RoleId.HasValue
                                ? guild.GetRole(roleAttribute.RoleId.Value)
                                : guild.Roles.FirstOrDefault(x => x.Name == roleAttribute.RoleName);
                        return (ownerCommand, role);
                    }
                )
                .Where(t => t.role != null)
                .Select(
                    t =>
                    {
                        var (ownerCommand, role) = t;
                        return _interactionService.ModifySlashCommandPermissionsAsync(
                            ownerCommand,
                            guild,
                            new ApplicationCommandPermission(role, true)
                        );
                    }
                );
            await Task.WhenAll(giveOwnerPermissions.Concat(giveRolePermissions));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while registering commands against a guild.");
        }
    }
}