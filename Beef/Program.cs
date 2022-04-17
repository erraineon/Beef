// See https://aka.ms/new-console-template for more information

using Beef.Core;
using Beef.Telegram;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Telegram.Bot;

Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(configurationBuilder => configurationBuilder.AddUserSecrets<Program>())
    .ConfigureServices(
        (context, services) =>
        {
            var discordSocketClient =
                new DiscordSocketClient(new DiscordSocketConfig { GatewayIntents = GatewayIntents.All });
            services
                .Configure<TelegramOptions>(context.Configuration.GetSection(nameof(TelegramOptions)))
                .AddHostedService<TelegramClientLauncher>()
                .AddSingleton<TelegramChatClient>()
                .AddSingleton<ITelegramBotClient>(
                    s => new TelegramBotClient(s.GetRequiredService<IOptions<TelegramOptions>>().Value.Token)
                )
                .AddTransient<ITelegramGuildCache, TelegramGuildCache>()
                .AddTransient<ITelegramGuildFactory, TelegramGuildFactory>()
                .AddTransient<ITelegramUserMessageFactory, TelegramUserMessageFactory>()
                .AddTransient<IUserMessageCache, UserMessageCache>()
                .Configure<DiscordOptions>(context.Configuration.GetSection(nameof(DiscordOptions)))
                .AddSingleton(discordSocketClient)
                .AddSingleton(
                    new InteractionService(
                        discordSocketClient,
                        new InteractionServiceConfig { DefaultRunMode = RunMode.Sync }
                    )
                )
                .AddTransient<IInteractionHandler, InteractionHandler>()
                .AddTransient<IInteractionFactory, InteractionFactory>()
                .AddHostedService<DiscordClientLauncher>()
                .AddMemoryCache();
        }
    )
    .Build()
    .Run();