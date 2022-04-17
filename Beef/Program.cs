// See https://aka.ms/new-console-template for more information

using Beef.Core;
using Beef.Core.Interactions;
using Beef.Telegram;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(configurationBuilder => configurationBuilder.AddUserSecrets<Program>())
    .ConfigureServices(
        (context, services) =>
        {
            var discordSocketClient =
                new DiscordSocketClient(new DiscordSocketConfig { GatewayIntents = GatewayIntents.All });
            services

                // Telegram
                .Configure<TelegramOptions>(context.Configuration.GetSection(nameof(TelegramOptions)))
                .AddSingleton<TelegramChatClient>()
                .AddTransient<ITelegramGuildCache, TelegramGuildCache>()
                .AddMemoryCache()
                .AddHostedService<TelegramClientLauncher>()

                // Discord
                .Configure<DiscordOptions>(context.Configuration.GetSection(nameof(DiscordOptions)))
                .AddSingleton(discordSocketClient)
                .AddHostedService<DiscordClientLauncher>()

                // Interactions
                .AddSingleton(
                    new InteractionService(
                        discordSocketClient,
                        new InteractionServiceConfig { DefaultRunMode = RunMode.Sync }
                    )
                )
                .AddTransient<IInteractionHandler, InteractionHandler>()
                .AddTransient<IInteractionFactory, InteractionFactory>()

                // Everything else
                ;
        }
    )
    .Build()
    .Run();