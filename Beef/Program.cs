// See https://aka.ms/new-console-template for more information

using Beef;
using Beef.Core;
using Beef.Core.Data;
using Beef.Core.Discord;
using Beef.Core.Interactions;
using Beef.Core.Telegram;
using Beef.Google;
using Beef.OpenAI;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Google.Apis.Customsearch.v1;
using Google.Apis.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OpenAI.GPT3.Managers;
using OpenAI.GPT3.Interfaces;
using IOpenAIService = Beef.OpenAI.IOpenAIService;
using OpenAIService = Beef.OpenAI.OpenAIService;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(configurationBuilder => configurationBuilder.AddUserSecrets<Program>())
    .ConfigureServices(
        (context, services) =>
        {
            // Interactions
            services
                .AddSingleton(
                    s => new InteractionService(
                        s.GetService<DiscordSocketClient>(),
                        new InteractionServiceConfig { DefaultRunMode = RunMode.Sync }
                    )
                )
                .AddTransient<IInteractionHandler, InteractionHandler>()
                .AddTransient<IInteractionFactory, InteractionFactory>()
                .AddHostedService<InteractionRegistrar>();

            // Discord
            services
                .Configure<DiscordOptions>(context.Configuration.GetSection(nameof(DiscordOptions)))
                .AddSingleton(
                    new DiscordSocketClient(
                        new DiscordSocketConfig { GatewayIntents = GatewayIntents.All }
                    )
                )
                .AddHostedService<DiscordClientLauncher>();

            // Telegram
            services
                .Configure<TelegramOptions>(context.Configuration.GetSection(nameof(TelegramOptions)))
                .AddSingleton<TelegramChatClient>()
                .AddTransient<ITelegramGuildCache, TelegramGuildCache>()
                .AddMemoryCache()
                .AddHostedService<TelegramClientLauncher>();

            // Triggers
            services
                .AddDbContextFactory<BeefDbContext>()
                .AddHostedService<TriggerListener>();

            // Whitelisting
            services
                .Configure<TrustedGuilds>(context.Configuration.GetSection(nameof(TrustedGuilds)));

            // Google
            services
                .Configure<GoogleOptions>(context.Configuration.GetSection(nameof(GoogleOptions)))
                .AddSingleton(
                    s => new CustomsearchService(
                        new BaseClientService.Initializer
                        {
                            ApiKey = s.GetRequiredService<IOptions<GoogleOptions>>().Value.ApiKey
                        }
                    )
                )
                .AddTransient<IGoogleSearchEngine, GoogleSearchEngine>();

            // OpenAi
            services
                .Configure<Gpt3Options>(context.Configuration.GetSection(nameof(Gpt3Options)))
                .Configure<OpenAiOptions>(context.Configuration.GetSection(nameof(OpenAiOptions)))
                .AddTransient<IOpenAIService, OpenAIService>()
                .AddSingleton<OpenAI.GPT3.Interfaces.IOpenAIService>(
                    s => new OpenAI.GPT3.Managers.OpenAIService(
                        new OpenAI.GPT3.OpenAiOptions
                        {
                            ApiKey = s.GetRequiredService<IOptions<OpenAiOptions>>().Value.ApiKey
                        }
                    )
                );
        }
    )
    .Build();

var dbContext = host.Services.GetRequiredService<BeefDbContext>();
dbContext.Database.Migrate();
host.Run();