namespace AUTOPAL.RTL.TVMaze.Services.TVShows.API
{
    using System;
    using System.Data.SqlClient;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Polly;

    public static class WebHostExtensions
    {
        public static IWebHost MigrateDbContext<TContext>(this IWebHost webHost,
            Action<TContext, IServiceProvider> seeder) where TContext : DbContext
        {
            using (var scope = webHost.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var logger = services.GetRequiredService<ILogger<TContext>>();

                var context = services.GetService<TContext>();

                try
                {
                    logger.LogInformation(
                        message: $"Migrating database associated with context {typeof(TContext).Name}");

                    var retry = Policy.Handle<SqlException>()
                        .WaitAndRetry(sleepDurations: new[]
                        {
                            TimeSpan.FromSeconds(value: 3),
                            TimeSpan.FromSeconds(value: 5),
                            TimeSpan.FromSeconds(value: 8)
                        });

                    retry.Execute(action: () =>
                    {
                        //if the sql server container is not created on run docker compose this
                        //migration can't fail for network related exception. The retry options for DbContext only 
                        //apply to transient exceptions.

                        context.Database
                            .Migrate();

                        seeder(arg1: context, arg2: services);
                    });


                    logger.LogInformation(
                        message: $"Migrated database associated with context {typeof(TContext).Name}");
                }
                catch (Exception ex)
                {
                    logger.LogError(exception: ex,
                        message:
                        $"An error occurred while migrating the database used on context {typeof(TContext).Name}");
                }
            }

            return webHost;
        }
    }
}