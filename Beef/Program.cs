// See https://aka.ms/new-console-template for more information

using Beef.Telegram;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

Host.CreateDefaultBuilder(args)
    .ConfigureServices(
        (context, services) => services
            .Configure<TelegramOptions>(context.Configuration.GetSection(nameof(TelegramOptions)))
            .AddHostedService<TelegramClientLauncher>()
    )
    .Build()
    .Run();