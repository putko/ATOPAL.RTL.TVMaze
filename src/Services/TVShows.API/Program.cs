using AUTOPOAL.RTL.TVMaze.Services.TVShows.API.Infrastructure;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.IO;

namespace AUTOPOAL.RTL.TVMaze.Services.TVShows.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHost(args)
                 .MigrateDbContext<TVShowContext>((context, services) =>
                 {
                 })
                .Run();
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
