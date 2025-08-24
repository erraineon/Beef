// See https://aka.ms/new-console-template for more information

using BabyGame;
using BabyGame.Data;
using BabyGame.Events;
using BabyGame.Models;
using BabyGame.Modifiers;
using BabyGame.Services;
using Beef;
using Beef.Core;
using Beef.Core.Data;
using Beef.Core.Discord;
using Beef.Core.Interactions;
using Beef.Core.Telegram;
using Beef.Furry;
using Beef.Google;
using Beef.OpenAi;
using Beef.TadmorMind;
using Betalgo.Ranul.OpenAI;
using Betalgo.Ranul.OpenAI.Interfaces;
using Betalgo.Ranul.OpenAI.Managers;
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
using NeoSmart.Caching.Sqlite;
using SQLitePCL;
using TimeProvider = BabyGame.Services.TimeProvider;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(configurationBuilder => configurationBuilder.AddUserSecrets<Program>())
    .ConfigureServices((context, services) =>
        {
            // Distributed cache
            services.AddSqliteCache("cache.db", new SQLite3Provider_e_sqlite3());

            // Interactions
            services
                .AddSingleton(s => new InteractionService(
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
                .AddSingleton(s => new CustomsearchService(
                        new BaseClientService.Initializer
                        {
                            ApiKey = s.GetRequiredService<IOptions<GoogleOptions>>().Value.ApiKey
                        }
                    )
                )
                .AddTransient<IGoogleSearchEngine, GoogleSearchEngine>();

            // OpenAi
            services
                .Configure<OpenAiOptions>(context.Configuration.GetSection(nameof(OpenAiOptions)))
                .AddTransient<IOpenAiService, OpenAiService>()
                .AddSingleton<IOpenAIService>(s => new OpenAIService(
                        new OpenAIOptions
                        {
                            ApiKey = s.GetRequiredService<IOptions<OpenAiOptions>>().Value.ApiKey
                        }
                    )
                );

            // E621
            services
                .AddTransient<IE621Client, E621ClientWrapper>()
                .AddTransient<IE621SearchEngine, E621SearchEngine>();

            // TadmorMind
            services
                .AddHttpClient()
                .Configure<TadmorMindOptions>(context.Configuration.GetSection(nameof(TadmorMindOptions)))
                .AddTransient<ITadmorMindClient, TadmorMindClient>()
                .AddTransient<IMessageContentPreprocessor, TadmorMindPreprocessor>()
                .AddSingleton<ITadmorMindThoughtsRepository, TadmorMindThoughtsRepository>()
                .AddHostedService<TadmorMindThoughtProducer>();

            // Baby game
            services
                .Configure<BabyGameOptions>(context.Configuration.GetSection(nameof(BabyGameOptions)))
                .AddDbContextFactory<BabyGameDbContext>()
                .AddScoped<IBabyGameRepository>(s => s.GetRequiredService<BabyGameDbContext>())
                .AddScoped<IBabyGameLogger, BabyGameLogger>()
                .AddTransient<IRandomProvider, RandomProvider>()
                .AddTransient<ITimeProvider, TimeProvider>()
                .AddTransient<IEventDispatcher, EventDispatcher>()
                .AddTransient<IBabyGachaService, BabyGachaService>()
                .AddTransient<IBabyLevelService, BabyLevelService>()
                .AddTransient<IKissService, KissService>()
                .AddTransient<IModifierService, ModifierService>()
                .AddTransient<IPetService, PetService>()
                .AddTransient<IMarriageService, MarriageService>()
                // baby services
                .AddTransient<IEventHandler<AffinityModifier, IKissComplete, int>, AffinityModifierService>()
                .AddTransient<IEventHandler<AffinityModifier, IKissCooldownMultiplierOnKiss, double>,
                    AffinityModifierService>()
                .AddTransient<IEventHandler<IMarriageComplete, int>, AffinityModifierService>()
                .AddTransient<IEventHandler<IMarriageComplete, int>, SkipLoveCostModifierService>();
        }
    )
    .Build();

var dbContext = host.Services.GetRequiredService<BeefDbContext>();
dbContext.Database.Migrate();
host.Run();