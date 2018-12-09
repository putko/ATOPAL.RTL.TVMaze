namespace AUTOPAL.RTL.TVMaze.Services.TVShows.API
{
    using System.IO;
    using AUTOPAL.RTL.TVMaze.Services.TVShows.API.Infrastructure;
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    public class Program
    {
        public static void Main(string[] args)
        {
            Program.CreateWebHost(args: args)
                .MigrateDbContext<TVShowContext>(seeder: (context, services) => { })
                .Run();
        }

        public static IWebHost CreateWebHost(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args: args)
                .UseStartup<Startup>()
                .UseContentRoot(contentRoot: Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration(configureDelegate: (builderContext, config) =>
                {
                    var builtConfig = config.Build();

                    var configurationBuilder = new ConfigurationBuilder();

                    configurationBuilder.AddEnvironmentVariables();

                    config.AddConfiguration(config: configurationBuilder.Build());
                })
                .ConfigureLogging(configureLogging: (hostingContext, builder) =>
                {
                    builder.AddConfiguration(configuration: hostingContext.Configuration.GetSection(key: "Logging"));
                    builder.AddConsole();
                    builder.AddDebug();
                }).Build();
        }
    }
}