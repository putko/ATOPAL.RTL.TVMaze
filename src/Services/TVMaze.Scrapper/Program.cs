namespace AUTOPAL.RTL.TVMaze.Services.TVMaze.Scrapper
{
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Logging;

    public class Program
    {
        public static void Main(string[] args)
        {
            Program.BuildWebHost(args: args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args: args)
                .UseStartup<Startup>()
                .ConfigureLogging(configureLogging: (hostingContext, builder) =>
                {
                    builder.AddConfiguration(configuration: hostingContext.Configuration.GetSection(key: "Logging"));
                    builder.AddDebug();
                    builder.AddConsole();
                }).Build();
        }
    }
}