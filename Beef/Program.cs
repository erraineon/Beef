// See https://aka.ms/new-console-template for more information

using Beef.Core.Chats;
using Beef.Core.Chats.Interactions.Execution;
using Beef.Core.Data;
using Beef.Core.Extensions;
using Beef.Core.Modules;
using Beef.Discord;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


Host.CreateDefaultBuilder(args)
    .ConfigureServices(
        (hostBuilderContext, services) =>
        {
            var typesToExclude = new List<Type>
            {
                typeof(BeefDbContext), typeof(ChatContext), typeof(BotInteraction), typeof(BotInteractionData),
                typeof(BotInteractionDataOption)
            };

            services
                .AddDbContext<IBeefDbContext, BeefDbContext>(
                    optionsBuilder => optionsBuilder.UseSqlite(
                        hostBuilderContext.Configuration.GetConnectionString(nameof(BeefDbContext))
                    )
                );

            services.AddScoped<IChatContext, ChatContext>();
            //services.AddScoped(s => s.GetRequiredService<IChatContext>().ChatClient);
            //services.AddScoped(s => s.GetRequiredService<IChatContext>().CommandRegistrar);


            services.Scan(
                x => x.FromAssemblyOf<ChatStartupService>()
                    .AddClasses(
                        f => f
                            // Don't register the modules.
                            .NotInNamespaceOf<TestModule>()
                            .Where(t => !typesToExclude.Contains(t))
                            .Where(ExcludeConcreteTypesAndRecords)
                    )
                    .AsImplementedInterfaces()
                    .WithTransientLifetime()
            );

            AddDiscord(services, hostBuilderContext);
        }
    )
    .Build()
    .Run();

void AddDiscord(
    IServiceCollection services,
    HostBuilderContext hostBuilderContext
)
{
    var discordOptions = hostBuilderContext.Configuration.GetFromSection<DiscordOptions>();
    if (discordOptions != null)
    {
        var singletonTypes = new[] { typeof(DiscordSocketClientWrapper), typeof(InteractionServiceWrapper) };
        var typesToExclude = new List<Type> { typeof(DiscordOptions) };

        services.AddSingleton<IDiscordOptions>(discordOptions);
        if (!hostBuilderContext.HostingEnvironment.IsProduction())
            typesToExclude.Add(typeof(DiscordGlobalCommandRegistrationService));

        services.Scan(
            x => x.FromAssemblyOf<DiscordOptions>()
                .AddClasses(
                    f => f
                        .Where(t => !singletonTypes.Contains(t))
                        .Where(t => !typesToExclude.Contains(t))
                        .Where(ExcludeConcreteTypesAndRecords)
                )
                .AsImplementedInterfaces()
                .WithTransientLifetime()
        );

        services.Scan(
            x => x.AddTypes(singletonTypes)
                .AsSelfWithInterfaces()
                .WithSingletonLifetime()
        );
    }
}

bool ExcludeConcreteTypesAndRecords(Type t) => t.GetInterfaces().Any() && t.GetMethods().All(m => m.Name != "<Clone>$");
