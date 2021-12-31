using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Beef.Core.Chats.Discord;
using Beef.Core.Extensions;
using Discord;
using Discord.Interactions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Beef.Tests;

[TestClass]
[TestCategory("Integration")]
public class DiscordTests
{
    private IConfigurationRoot _configuration;
    
    [TestInitialize]
    public void TestInitialize()
    {
        _configuration = new ConfigurationBuilder()
            .AddUserSecrets<DiscordTests>()
            .Build();
    }

    [TestMethod]
    public async Task Login_OK()
    {
        var discordOptions = _configuration.GetFromSection<DiscordOptions>();

        var client = new DiscordSocketClientWrapper();

        var discordLauncher = new DiscordLauncher(
            discordOptions,
            client
        );

        await discordLauncher.StartAsync(CancellationToken.None);

        var applicationInfo = await client.GetApplicationInfoAsync();
        await applicationInfo.Owner.SendMessageAsync($"Test {nameof(Login_OK)} successful.");

        await client.StopAsync();
    }

    [TestMethod]
    public async Task GuildCommands_Registration_OK()
    {
        var discordOptions = _configuration.GetFromSection<DiscordOptions>();

        var client = new DiscordSocketClientWrapper();

        var discordLauncher = new DiscordLauncher(
            discordOptions,
            client
        );

        var interactionService = new InteractionServiceWrapper(client);

        var chatService = new ChatService(
            new ModuleRegistrar(new[] { interactionService }, new ServiceCollection().BuildServiceProvider()),
            new[] { discordLauncher },
            new[] { new GuildSpecificCommandRegistrar(discordOptions, interactionService) });

        await chatService.StartAsync(CancellationToken.None);

        Assert.IsTrue(interactionService.Modules.Any(m => m.SlashCommands.Any(c => c.Name == "ping")));

        await chatService.StopAsync(CancellationToken.None);
    }

    [TestMethod]
    public async Task Interaction_Interception_OK()
    {
        var discordOptions = _configuration.GetFromSection<DiscordOptions>();

        var client = new DiscordSocketClientWrapper();

        var discordLauncher = new DiscordLauncher(
            discordOptions,
            client
        );

        var interactionService = new InteractionServiceWrapper(client);

        var chatService = new ChatService(
            new ModuleRegistrar(new[] { interactionService }, new ServiceCollection().BuildServiceProvider()),
            new[] { discordLauncher },
            new[] { new GuildSpecificCommandRegistrar(discordOptions, interactionService) });

        await chatService.StartAsync(CancellationToken.None);

        var interaction = await InteractionUtility.WaitForInteractionAsync(
            client,
            TimeSpan.FromMinutes(1),
            x => x is IApplicationCommandInteraction { Data.Name: "ping" });
        await interaction.RespondAsync("pong");

        await chatService.StopAsync(CancellationToken.None);
    }

    [TestMethod]
    public async Task Interaction_Module_Interception_OK()
    {
        var discordOptions = _configuration.GetFromSection<DiscordOptions>();

        var client = new DiscordSocketClientWrapper();

        var discordLauncher = new DiscordLauncher(
            discordOptions,
            client
        );

        var interactionService = new InteractionServiceWrapper(client);

        var chatService = new ChatService(
            new ModuleRegistrar(new[] { interactionService }, new ServiceCollection().BuildServiceProvider()),
            new[] { discordLauncher },
            new[] { new GuildSpecificCommandRegistrar(discordOptions, interactionService) });

        await chatService.StartAsync(CancellationToken.None);

        // TODO: hook the execution of the ping command via module

        await chatService.StopAsync(CancellationToken.None);
    }
}