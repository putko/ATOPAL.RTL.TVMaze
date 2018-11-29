using Autofac;
using Autofac.Extensions.DependencyInjection;
using BuildingBlocks.EventBus.Abstractions;
using BuildingBlocks.IntegrationEventLogEF;
using BuildingBlocks.IntegrationEventLogEF.Services;
using EventBusRabbitMQ;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System;
using System.Data.Common;
using System.Reflection;
using TVShows.API.Infrastructure;
using TVShows.API.Infrastructure.Filters;
using TVShows.API.IntegrationEvents;
using TVShows.API.IntegrationEvents.EventHandling;
using TVShows.API.IntegrationEvents.Events;
using TVShows.API.Model;

namespace TVShows.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddCustomMVC(Configuration)
                    .AddCustomDbContext(Configuration)
                    .AddSwagger();

            services.AddTransient<DbContext, TVShowContext>();
            services.AddTransient<IGenericRepository<Show>, GenericRepository<Show>>();

            ContainerBuilder container = new ContainerBuilder();
            container.Populate(services);
            return new AutofacServiceProvider(container.Build());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            
            var pathBase = Configuration["PATH_BASE"];

            if (!string.IsNullOrEmpty(pathBase))
            {
                app.UsePathBase(pathBase);
            }

            app.UseCors("CorsPolicy");

            app.UseMvcWithDefaultRoute();

            app.UseSwagger()
              .UseSwaggerUI(c =>
              {
                  c.SwaggerEndpoint($"{ (!string.IsNullOrEmpty(pathBase) ? pathBase : string.Empty) }/swagger/v1/swagger.json", "Catalog.API V1");
              });



            ConfigureEventBus(app);
        }
        protected virtual void ConfigureEventBus(IApplicationBuilder app)
        {
            //var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
            //eventBus.Subscribe<ShowUpdatedIntegrationEvent, ShowUpdatedIntegrationEventHandler>();
        }
    }

    public static class CustomExtensionMethods
    {
        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.DescribeAllEnumsAsStrings();
                options.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info
                {
                    Title = "RTL-TVMaze - TVShows HTTP API",
                    Version = "v1",
                    Description = "The TVShows Microservice HTTP API. This is a Data-Driven/CRUD microservice sample",
                    TermsOfService = "Terms Of Service"
                });
            });

            return services;

        }

        public static IServiceCollection AddCustomDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<TVShowContext>(options =>
            {
                options.UseSqlServer(configuration["ConnectionString"],
                                     sqlServerOptionsAction: sqlOptions =>
                                     {
                                         sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                                         //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency 
                                         sqlOptions.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                                     });

                // Changing default behavior when client evaluation occurs to throw. 
                // Default in EF Core would be to log a warning when client evaluation is performed.
                options.ConfigureWarnings(warnings => warnings.Throw(RelationalEventId.QueryClientEvaluationWarning));
                //Check Client vs. Server evaluation: https://docs.microsoft.com/en-us/ef/core/querying/client-eval
            });

            services.AddDbContext<IntegrationEventLogContext>(options =>
            {
                options.UseSqlServer(configuration["ConnectionString"],
                                     sqlServerOptionsAction: sqlOptions =>
                                     {
                                         sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                                         //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency 
                                         sqlOptions.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                                     });
            });

            return services;
        }

        public static IServiceCollection AddCustomMVC(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(HttpGlobalExceptionFilter));
            }).AddControllersAsServices();

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });

            return services;
        }



        //public static IServiceCollection AddCustomOptions(this IServiceCollection services, IConfiguration configuration)
        //{
        //    services.Configure<CatalogSettings>(configuration);
        //    services.Configure<ApiBehaviorOptions>(options =>
        //    {
        //        options.InvalidModelStateResponseFactory = context =>
        //        {
        //            var problemDetails = new ValidationProblemDetails(context.ModelState)
        //            {
        //                Instance = context.HttpContext.Request.Path,
        //                Status = StatusCodes.Status400BadRequest,
        //                Detail = "Please refer to the errors property for additional details."
        //            };

        //            return new BadRequestObjectResult(problemDetails)
        //            {
        //                ContentTypes = { "application/problem+json", "application/problem+xml" }
        //            };
        //        };
        //    });

        //    return services;
        //}



        public static IServiceCollection AddIntegrationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<Func<DbConnection, IIntegrationEventLogService>>(
                sp => (DbConnection c) => new IntegrationEventLogService(c));

            services.AddTransient<ITVShowsIntegrationEventService, TVShowsIntegrationEventService>();

            services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();

                var factory = new ConnectionFactory()
                {
                    HostName = configuration["EventBusConnection"]
                };

                if (!string.IsNullOrEmpty(configuration["EventBusUserName"]))
                {
                    factory.UserName = configuration["EventBusUserName"];
                }

                if (!string.IsNullOrEmpty(configuration["EventBusPassword"]))
                {
                    factory.Password = configuration["EventBusPassword"];
                }

                int retryCount = 5;
                if (!string.IsNullOrEmpty(configuration["EventBusRetryCount"]))
                {
                    retryCount = int.Parse(configuration["EventBusRetryCount"]);
                }

                return new DefaultRabbitMQPersistentConnection(factory, logger, retryCount);
            });

            return services;
        }

        //public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration)
        //{
        //    var subscriptionClientName = configuration["SubscriptionClientName"];

        //    if (configuration.GetValue<bool>("AzureServiceBusEnabled"))
        //    {
        //        services.AddSingleton<IEventBus, EventBusServiceBus>(sp =>
        //        {
        //            var serviceBusPersisterConnection = sp.GetRequiredService<IServiceBusPersisterConnection>();
        //            var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
        //            var logger = sp.GetRequiredService<ILogger<EventBusServiceBus>>();
        //            var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

        //            return new EventBusServiceBus(serviceBusPersisterConnection, logger,
        //                eventBusSubcriptionsManager, subscriptionClientName, iLifetimeScope);
        //        });

        //    }
        //    else
        //    {
        //        services.AddSingleton<IEventBus, EventBusRabbitMQ>(sp =>
        //        {
        //            var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
        //            var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
        //            var logger = sp.GetRequiredService<ILogger<EventBusRabbitMQ>>();
        //            var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

        //            var retryCount = 5;
        //            if (!string.IsNullOrEmpty(configuration["EventBusRetryCount"]))
        //            {
        //                retryCount = int.Parse(configuration["EventBusRetryCount"]);
        //            }

        //            return new EventBusRabbitMQ(rabbitMQPersistentConnection, logger, iLifetimeScope, eventBusSubcriptionsManager, subscriptionClientName, retryCount);
        //        });
        //    }

        //    services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
        //    services.AddTransient<OrderStatusChangedToAwaitingValidationIntegrationEventHandler>();
        //    services.AddTransient<OrderStatusChangedToPaidIntegrationEventHandler>();

        //    return services;
        //}
    }
}
