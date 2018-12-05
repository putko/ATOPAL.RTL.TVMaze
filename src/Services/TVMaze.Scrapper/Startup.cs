using Autofac;
using Autofac.Extensions.DependencyInjection;
using AUTOPOAL.RTL.TVMaze.BuildingBlocks.EventBus.Common;
using AUTOPOAL.RTL.TVMaze.BuildingBlocks.EventBus.Common.Abstractions;
using AUTOPOAL.RTL.TVMaze.BuildingBlocks.EventBus.RabbitMQ;
using AUTOPOAL.RTL.TVMaze.Services.TVMaze.Scrapper.Configuration;
using AUTOPOAL.RTL.TVMaze.Services.TVMaze.Scrapper.Model;
using AUTOPOAL.RTL.TVMaze.Services.TVMaze.Scrapper.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using StackExchange.Redis;
using System;
using System.Data.Common;

namespace AUTOPOAL.RTL.TVMaze.Services.TVMaze.Scrapper
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
            //configure settings

            services.Configure<BackgroundTaskSettings>(Configuration);
            services.AddOptions()
                    .AddCustomDbContext(Configuration)
                    .AddIntegrationServices(Configuration)
                    .AddEventBus(Configuration);
            
            
            //configure background task

            services.AddSingleton<IHostedService, TVMazeScrapperService>();
            services.AddTransient<TVMazeUpdater>();

            services.Configure<ScrapperSettings>(Configuration);



            //configure event bus related services

            services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
            {
                ILogger<DefaultRabbitMQPersistentConnection> logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();


                ConnectionFactory factory = new ConnectionFactory()
                {
                    HostName = Configuration["EventBusConnection"]
                };

                if (!string.IsNullOrEmpty(Configuration["EventBusUserName"]))
                {
                    factory.UserName = Configuration["EventBusUserName"];
                }

                if (!string.IsNullOrEmpty(Configuration["EventBusPassword"]))
                {
                    factory.Password = Configuration["EventBusPassword"];
                }

                int retryCount = 5;
                if (!string.IsNullOrEmpty(Configuration["EventBusRetryCount"]))
                {
                    retryCount = int.Parse(Configuration["EventBusRetryCount"]);
                }

                return new DefaultRabbitMQPersistentConnection(factory, logger, retryCount);
            });
            services.AddTransient<IConcurrencyRepository, RedisConcurrencyRepository>();

            RegisterEventBus(services);

            //create autofac based service provider
            ContainerBuilder container = new ContainerBuilder();
            container.Populate(services);


            return new AutofacServiceProvider(container.Build());
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
        }


        private void RegisterEventBus(IServiceCollection services)
        {
            string subscriptionClientName = Configuration["SubscriptionClientName"];


            services.AddSingleton<IEventBus, EventBusRabbitMQ>(sp =>
            {
                IRabbitMQPersistentConnection rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
                ILifetimeScope iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
                ILogger<EventBusRabbitMQ> logger = sp.GetRequiredService<ILogger<EventBusRabbitMQ>>();
                IEventBusSubscriptionsManager eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

                int retryCount = 5;
                if (!string.IsNullOrEmpty(Configuration["EventBusRetryCount"]))
                {
                    retryCount = int.Parse(Configuration["EventBusRetryCount"]);
                }

                return new EventBusRabbitMQ(rabbitMQPersistentConnection, logger, iLifetimeScope, eventBusSubcriptionsManager, subscriptionClientName, retryCount);
            });

            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
        }
    }
    public static class CustomExtensionMethods
    {

        public static IServiceCollection AddCustomDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            //By connecting here we are making sure that our service
            //cannot start until redis is ready. This might slow down startup,
            //but given that there is a delay on resolving the ip address
            //and then creating the connection it seems reasonable to move
            //that cost to startup instead of having the first request pay the
            //penalty.
            services.AddSingleton<ConnectionMultiplexer>(sp =>
            {
                ScrapperSettings settings = sp.GetRequiredService<IOptions<ScrapperSettings>>().Value;
                ConfigurationOptions configurationOptions = ConfigurationOptions.Parse(settings.ConnectionString, true);

                configurationOptions.ResolveDns = true;

                return ConnectionMultiplexer.Connect(configurationOptions);
            });
            return services;
        }

        public static IServiceCollection AddIntegrationServices(this IServiceCollection services, IConfiguration configuration)
        {
           services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
            {
                ILogger<DefaultRabbitMQPersistentConnection> logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();

                ConnectionFactory factory = new ConnectionFactory()
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

        public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration)
        {
            string subscriptionClientName = configuration["SubscriptionClientName"];

            services.AddSingleton<IEventBus, EventBusRabbitMQ>(sp =>
            {
                IRabbitMQPersistentConnection rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
                ILifetimeScope iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
                ILogger<EventBusRabbitMQ> logger = sp.GetRequiredService<ILogger<EventBusRabbitMQ>>();
                IEventBusSubscriptionsManager eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

                int retryCount = 5;
                if (!string.IsNullOrEmpty(configuration["EventBusRetryCount"]))
                {
                    retryCount = int.Parse(configuration["EventBusRetryCount"]);
                }

                return new EventBusRabbitMQ(rabbitMQPersistentConnection, logger, iLifetimeScope, eventBusSubcriptionsManager, subscriptionClientName, retryCount);
            });

            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();

            return services;
        }
    }
}
