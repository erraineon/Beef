// See https://aka.ms/new-console-template for more information

using Beef;
using Beef.Core;
using Beef.Core.Data;
using Beef.Core.Interactions;
using Beef.Core.Telegram;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(configurationBuilder => configurationBuilder.AddUserSecrets<Program>())
    .ConfigureServices(
        (context, services) =>
        {
            // Telegram
            services
                .Configure<TelegramOptions>(context.Configuration.GetSection(nameof(TelegramOptions)))
                .AddSingleton<TelegramChatClient>()
                .AddTransient<ITelegramGuildCache, TelegramGuildCache>()
                .AddMemoryCache()
                .AddHostedService<TelegramClientLauncher>();

            // Discord
            services
                .Configure<DiscordOptions>(context.Configuration.GetSection(nameof(DiscordOptions)))
                .AddSingleton(
                    new DiscordSocketClient(
                        new DiscordSocketConfig { GatewayIntents = GatewayIntents.All }
                    )
                )
                .AddHostedService<DiscordClientLauncher>();

            // Interactions
            services
                .AddSingleton(
                    s => new InteractionService(
                        s.GetService<DiscordSocketClient>(),
                        new InteractionServiceConfig { DefaultRunMode = RunMode.Sync }
                    )
                )
                .AddTransient<IInteractionHandler, InteractionHandler>()
                .AddTransient<IInteractionFactory, InteractionFactory>();

            // Everything else
            services.AddHostedService<TriggerListener>();
            services.AddDbContextFactory<BeefDbContext>();
        }
    )
    .Build();

var dbContext = host.Services.GetRequiredService<BeefDbContext>();
dbContext.Database.MigrateAsync();
host.Run();