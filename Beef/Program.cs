// See https://aka.ms/new-console-template for more information

using Beef.Core;
using Beef.Core.Chats;
using Beef.Core.Chats.Interactions.Execution;
using Beef.Core.Chats.Interactions.Registration;
using Beef.Core.Data;
using Beef.Core.Triggers;
using Beef.Discord;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = CreateHostBuilder(args).Build();
Migrate(host);
host.Run();

static void Migrate(IHost host)
{
    using var scope = host.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<IBeefDbContext>();
    dbContext.Database.Migrate();
}

static IHostBuilder CreateHostBuilder(string[] strings)
{
    return Host.CreateDefaultBuilder(strings)
        .ConfigureServices(
            (hostBuilderContext, services) =>
            {
                // Database
                services
                    .AddDbContext<IBeefDbContext, BeefDbContext>(
                        optionsBuilder => optionsBuilder.UseSqlite(
                            hostBuilderContext.Configuration.GetConnectionString(nameof(BeefDbContext))
                        )
                    );

                // Core interactions and commands
                services.AddTransient<IInteractionExecutor, InteractionExecutor>();
                services.AddTransient<IInteractionFactory, InteractionFactory>();
                services.AddTransient<IInteractionHandler, InteractionHandler>();
                services.AddTransient<IModuleRegistrar, ModuleRegistrar>();
                services.AddHostedService<ChatStartupService>();

                // Triggers
                services.AddTransient<ITimeTriggerFactory, TimeTriggerFactory>();
                services.AddTransient<ITriggerExecutor, TriggerExecutor>();
                services.AddHostedService<TimeTriggerListener>();

                // Utilities
                services.AddScoped<IChatContextHolder, ChatContextHolder>();
                services.AddTransient(s => s.GetRequiredService<IChatContextHolder>().Context);
                services.AddTransient<IChatScopeFactory, ChatScopeFactory>();
                services.AddTransient<ICurrentTimeProvider, CurrentTimeProvider>();

                AddDiscord(services, hostBuilderContext);
            }
        );

    void AddDiscord(
        IServiceCollection services,
        HostBuilderContext hostBuilderContext
    )
    {
        var discordOptions = hostBuilderContext.Configuration.GetFromSection<DiscordOptions>();
        if (discordOptions != null)
        {
            // Options
            services.AddSingleton<IDiscordOptions>(discordOptions);

            // Discord chat services
            services.AddSingleton<DiscordSocketClientWrapper>();
            services.AddSingleton<IDiscordChatClient>(s => s.GetRequiredService<DiscordSocketClientWrapper>());
            services.AddSingleton<IChatClient>(s => s.GetRequiredService<DiscordSocketClientWrapper>());
            services.AddSingleton<IChatComponent>(s => s.GetRequiredService<DiscordSocketClientWrapper>());

            services.AddSingleton<InteractionServiceWrapper>();
            services.AddSingleton<IInteractionService>(s => s.GetRequiredService<InteractionServiceWrapper>());
            services.AddSingleton<IDiscordCommandRegistrar>(s => s.GetRequiredService<InteractionServiceWrapper>());
            services.AddSingleton<ICommandRegistrar>(s => s.GetRequiredService<InteractionServiceWrapper>());
            services.AddSingleton<IChatComponent>(s => s.GetRequiredService<InteractionServiceWrapper>());

            services.AddHostedService<DiscordInteractionListener>();
            services.AddTransient<IChatClientLauncher, DiscordLauncher>();

            // Command registration services
            services.AddTransient<ICommandRegistrationService, DiscordGuildsCommandRegistrationService>();
            if (hostBuilderContext.HostingEnvironment.IsProduction())
                services.AddTransient<ICommandRegistrationService, DiscordGlobalCommandRegistrationService>();
        }
    }
}