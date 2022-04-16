// See https://aka.ms/new-console-template for more information

using Beef.Discord;
using Beef.Telegram;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(configurationBuilder => configurationBuilder.AddUserSecrets<Program>())
    .ConfigureServices(
        (context, services) => services
            .Configure<TelegramOptions>(context.Configuration.GetSection(nameof(TelegramOptions)))
            .AddHostedService<TelegramClientLauncher>()
            .Configure<DiscordOptions>(context.Configuration.GetSection(nameof(DiscordOptions)))
            .AddHostedService<DiscordClientLauncher>()
    )
    .Build()
    .Run();