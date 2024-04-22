﻿using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetCore.AutoRegisterDi;

var configDiscord = new DiscordSocketConfig
{
    AlwaysDownloadUsers = true,
    GatewayIntents = GatewayIntents.MessageContent | GatewayIntents.GuildMembers
};

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(config =>
    {
        config.SetBasePath(Directory.GetCurrentDirectory());
        config.AddEnvironmentVariables();
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices(services =>
    {
        services.AddSingleton(new DiscordSocketClient(configDiscord)); // Add the discord client to services
        services.AddSingleton<InteractionService>(); // Add the interaction service to services

        // Register any class that ends with "Service" as a service
        services.RegisterAssemblyPublicNonGenericClasses()
            .Where(c => c.Name.EndsWith("Service"))
            .AsPublicImplementedInterfaces();
    })
    .Build();

await host.RunAsync();
