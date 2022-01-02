// See https://aka.ms/new-console-template for more information

using Beef.Core.Chats;
using Beef.Core.Extensions;
using Beef.Core.Modules;
using Beef.Discord;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

Host.CreateDefaultBuilder(args)
    .ConfigureServices(
        (hostBuilderContext, s) =>
        {
            var c = hostBuilderContext.Configuration;
            // Core
            s.Scan(
                x => x.FromAssemblyOf<ChatStartupService>()
                    .AddClasses(f => f.NotInNamespaceOf<CommandResult>())
                    .AsImplementedInterfaces()
                    .WithTransientLifetime()
            );

            // Discord
            var discordOptions = c.GetFromSection<DiscordOptions>();
            if (discordOptions != null)
            {
                var singletonTypes = new[] { typeof(DiscordSocketClientWrapper), typeof(InteractionServiceWrapper) };
                var typesToExclude = new List<Type> { typeof(DiscordOptions) };

                s.AddSingleton<IDiscordOptions>(discordOptions);
                if (!hostBuilderContext.HostingEnvironment.IsProduction())
                    typesToExclude.Add(typeof(DiscordGlobalCommandRegistrationService));

                s.Scan(
                    x => x.FromAssemblyOf<DiscordOptions>()
                        .AddClasses(f => f
                            .Where(t => !singletonTypes.Contains(t))
                            .Where(t => !typesToExclude.Contains(t))
                        )
                        .AsImplementedInterfaces()
                        .WithTransientLifetime()
                );

                s.Scan(
                    x => x.AddTypes(singletonTypes)
                        .AsSelfWithInterfaces()
                        .WithSingletonLifetime()
                );
            }
        }
    )
    .Build()
    .Run();