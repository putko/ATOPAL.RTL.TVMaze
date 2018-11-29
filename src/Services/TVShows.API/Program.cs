using BuildingBlocks.IntegrationEventLogEF;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.IO;
using TVShows.API.Infrastructure;

namespace TVShows.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHost(args)
                 .MigrateDbContext<TVShowContext>((context, services) =>
                 {
                 })
                .MigrateDbContext<IntegrationEventLogContext>((_, __) => { }).Run();
        }

        public static IWebHost CreateWebHost(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
               .UseStartup<Startup>()
                  .UseContentRoot(Directory.GetCurrentDirectory())
                  .ConfigureAppConfiguration((builderContext, config) =>
                  {
                      IConfigurationRoot builtConfig = config.Build();

                      ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();

                      configurationBuilder.AddEnvironmentVariables();

                      config.AddConfiguration(configurationBuilder.Build());
                  })
                  .ConfigureLogging((hostingContext, builder) =>
                  {
                      builder.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                      builder.AddConsole();
                      builder.AddDebug();
                  }).Build();
        }
    }
}
