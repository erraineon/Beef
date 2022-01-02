using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Beef.Core.Chats;
using Beef.Core.Chats.Interactions.Execution;
using Beef.Core.Chats.Interactions.Registration;
using Beef.Core.Extensions;
using Beef.Core.Modules;
using Beef.Discord;
using Discord;
using Discord.Interactions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Beef.Tests;

[TestClass]
[TestCategory("Integration")]
public class DiscordTests
{
    private DiscordOptions _discordOptions;

    [TestInitialize]
    public void TestInitialize()
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<DiscordTests>()
            .Build();
        _discordOptions = configuration.GetFromSection<DiscordOptions>() ??
            throw new Exception("Integration tests configuration not found.");
    }

    private async Task WithClientAsync(Func<DiscordSocketClientWrapper, InteractionServiceWrapper, Task> action)
    {
        var client = new DiscordSocketClientWrapper();

        var interactionService = new InteractionServiceWrapper(client);

        var discordLauncher = new DiscordLauncher(
            _discordOptions,
            client
        );

        var moduleRegistrar = new ModuleRegistrar(interactionService, new ServiceCollection().BuildServiceProvider());

        var chatClientLauncher = new ChatStartupService(
            new[] { discordLauncher },
            new[] { new DiscordGuildsCommandRegistrationService(client, interactionService) },
            moduleRegistrar
        );
        try
        {
            await chatClientLauncher.StartAsync(CancellationToken.None);
            await action(client, interactionService);
        }
        finally
        {
            await chatClientLauncher.StopAsync(CancellationToken.None);
        }

    }

    [TestMethod]
    public async Task Interaction_Interception_OK()
    {
        await WithClientAsync(
            async (client, _) =>
            {
                var interaction = await InteractionUtility.WaitForInteractionAsync(
                    client,
                    TimeSpan.FromMinutes(1),
                    x => x is IApplicationCommandInteraction { Data.Name: "test" } c &&
                        c.Data.Options.Count(o => o.Name == "ping") == 1
                );
                await interaction.RespondAsync("Pong from interaction interception integration test");
            }
        );
    }

    [TestMethod]
    public async Task Interaction_Module_Interception_OK()
    {
        await WithClientAsync(
            async (client, interactionService) =>
            {
                var services = new ServiceCollection().BuildServiceProvider();
                var interactionExecutor = Substitute.For<IInteractionExecutor>();
                var tcs = new TaskCompletionSource<IResult>();
                interactionExecutor
                    .ExecuteInteractionAsync(Arg.Any<IInteractionContext>())
                    .Returns(
                        async x =>
                        {
                            var r = await interactionService.ExecuteCommandAsync(
                                x.Arg<IInteractionContext>(),
                                services
                            );
                            tcs.TrySetResult(r);
                            return r;
                        }
                    );

                var interactionListener = new DiscordInteractionListener(
                    client,
                    new InteractionHandler(interactionExecutor, Substitute.For<ILogger<InteractionHandler>>())
                );

                await interactionListener.StartAsync(CancellationToken.None);

                var cts = new CancellationTokenSource(TimeSpan.FromMinutes(1));
                cts.Token.Register(() => tcs.TrySetCanceled());
                var result = await tcs.Task;
                Assert.AreEqual("Pong", (result as CommandResult)?.Result);
            }
        );
    }
}