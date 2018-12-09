namespace AUTOPAL.RTL.TVMaze.Services.TVShows.API
{
    using System;
    using System.Reflection;
    using Autofac;
    using AUTOPAL.RTL.TVMaze.BuildingBlocks.EventBus.Common;
    using AUTOPAL.RTL.TVMaze.BuildingBlocks.EventBus.Common.Abstractions;
    using AUTOPAL.RTL.TVMaze.BuildingBlocks.EventBus.RabbitMQ;
    using AUTOPAL.RTL.TVMaze.Services.TVShows.API.Infrastructure;
    using AUTOPAL.RTL.TVMaze.Services.TVShows.API.Infrastructure.Filters;
    using AUTOPAL.RTL.TVMaze.Services.TVShows.API.IntegrationEvents.EventHandling;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Diagnostics;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using RabbitMQ.Client;
    using Swashbuckle.AspNetCore.Swagger;

    public static class StartUpExtensionMethods
    {
        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(setupAction: options =>
            {
                options.DescribeAllEnumsAsStrings();
                options.SwaggerDoc(name: "v1", info: new Info
                {
                    Title = "RTL-TVMaze - TVShows HTTP API",
                    Version = "v1",
                    Description = "The TVShows Micro service HTTP API. This is a Data-Driven/CRUD micro service sample",
                    TermsOfService = "Terms Of Service"
                });
            });

            return services;
        }

        public static IServiceCollection AddCustomDbContext(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<TVShowContext>(optionsAction: options =>
            {
                options.UseSqlServer(connectionString: configuration[key: "ConnectionString"],
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(assemblyName: typeof(Startup).GetTypeInfo().Assembly.GetName()
                            .Name);
                        //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency 
                        sqlOptions.EnableRetryOnFailure(maxRetryCount: 10,
                            maxRetryDelay: TimeSpan.FromSeconds(value: 30), errorNumbersToAdd: null);
                    });

                // Changing default behavior when client evaluation occurs to throw. 
                // Default in EF Core would be to log a warning when client evaluation is performed.
                options.ConfigureWarnings(warningsConfigurationBuilderAction: warnings =>
                    warnings.Throw(RelationalEventId.QueryClientEvaluationWarning));
                //Check Client vs. Server evaluation: https://docs.microsoft.com/en-us/ef/core/querying/client-eval
            });
            return services;
        }

        public static IServiceCollection AddCustomMvc(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMvc(setupAction: options =>
                {
                    options.Filters.Add(filterType: typeof(HttpGlobalExceptionFilter));
                })
                .AddControllersAsServices();

            services.AddCors(setupAction: options =>
            {
                options.AddPolicy(name: "CorsPolicy",
                    configurePolicy: builder => builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });

            return services;
        }


        public static IServiceCollection AddCustomOptions(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<ApiBehaviorOptions>(configureOptions: options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var problemDetails = new ValidationProblemDetails(modelState: context.ModelState)
                    {
                        Instance = context.HttpContext.Request.Path,
                        Status = StatusCodes.Status400BadRequest,
                        Detail = "Please refer to the errors property for additional details."
                    };

                    return new BadRequestObjectResult(error: problemDetails)
                    {
                        ContentTypes = {"application/problem+json", "application/problem+xml"}
                    };
                };
            });

            return services;
        }

        public static IServiceCollection AddIntegrationServices(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddSingleton<IRabbitMqPersistentConnection>(implementationFactory: sp =>
            {
                var logger = sp.GetRequiredService<ILogger<DefaultRabbitMqPersistentConnection>>();

                var factory = new ConnectionFactory
                {
                    HostName = configuration[key: "EventBusConnection"]
                };

                if (!string.IsNullOrEmpty(value: configuration[key: "EventBusUserName"]))
                {
                    factory.UserName = configuration[key: "EventBusUserName"];
                }

                if (!string.IsNullOrEmpty(value: configuration[key: "EventBusPassword"]))
                {
                    factory.Password = configuration[key: "EventBusPassword"];
                }

                var retryCount = 5;
                if (!string.IsNullOrEmpty(value: configuration[key: "EventBusRetryCount"]))
                {
                    retryCount = int.Parse(s: configuration[key: "EventBusRetryCount"]);
                }

                return new DefaultRabbitMqPersistentConnection(connectionFactory: factory, logger: logger,
                    retryCount: retryCount);
            });

            return services;
        }

        public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration)
        {
            var subscriptionClientName = configuration[key: "SubscriptionClientName"];

            services.AddSingleton<IEventBus, EventBusRabbitMq>(implementationFactory: sp =>
            {
                var rabbitMqPersistentConnection = sp.GetRequiredService<IRabbitMqPersistentConnection>();
                var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
                var logger = sp.GetRequiredService<ILogger<EventBusRabbitMq>>();
                var eventBusSubscriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

                var retryCount = 5;
                if (!string.IsNullOrEmpty(value: configuration[key: "EventBusRetryCount"]))
                {
                    retryCount = int.Parse(s: configuration[key: "EventBusRetryCount"]);
                }

                return new EventBusRabbitMq(persistentConnection: rabbitMqPersistentConnection, logger: logger,
                    container: iLifetimeScope,
                    subsManager: eventBusSubscriptionsManager, queueName: subscriptionClientName,
                    retryCount: retryCount);
            });

            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
            services.AddTransient<ShowUpdatedIntegrationEventHandler>();

            return services;
        }
    }
}