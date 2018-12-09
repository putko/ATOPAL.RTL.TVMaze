namespace AUTOPAL.RTL.TVMaze.Services.TVMaze.Scrapper
{
    using System;
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using AUTOPAL.RTL.TVMaze.BuildingBlocks.EventBus.Common;
    using AUTOPAL.RTL.TVMaze.BuildingBlocks.EventBus.Common.Abstractions;
    using AUTOPAL.RTL.TVMaze.BuildingBlocks.EventBus.RabbitMQ;
    using AUTOPAL.RTL.TVMaze.Services.TVMaze.Scrapper.Configuration;
    using AUTOPAL.RTL.TVMaze.Services.TVMaze.Scrapper.Model;
    using AUTOPAL.RTL.TVMaze.Services.TVMaze.Scrapper.Tasks;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using RabbitMQ.Client;
    using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            //configure settings

            services.Configure<BackgroundTaskSettings>(config: this.Configuration);
            services.AddOptions()
                .AddCustomDbContext(configuration: this.Configuration)
                .AddIntegrationServices(configuration: this.Configuration)
                .AddEventBus(configuration: this.Configuration);


            //configure background task

            services.AddSingleton<IHostedService, TvMazeScrapperService>();
            services.AddTransient<TvMazeUpdater>();

            services.Configure<ScrapperSettings>(config: this.Configuration);


            //configure event bus related services

            services.AddSingleton<IRabbitMqPersistentConnection>(implementationFactory: sp =>
            {
                var logger = sp.GetRequiredService<ILogger<DefaultRabbitMqPersistentConnection>>();


                var factory = new ConnectionFactory
                {
                    HostName = this.Configuration[key: "EventBusConnection"]
                };

                if (!string.IsNullOrEmpty(value: this.Configuration[key: "EventBusUserName"]))
                {
                    factory.UserName = this.Configuration[key: "EventBusUserName"];
                }

                if (!string.IsNullOrEmpty(value: this.Configuration[key: "EventBusPassword"]))
                {
                    factory.Password = this.Configuration[key: "EventBusPassword"];
                }

                var retryCount = 5;
                if (!string.IsNullOrEmpty(value: this.Configuration[key: "EventBusRetryCount"]))
                {
                    retryCount = int.Parse(s: this.Configuration[key: "EventBusRetryCount"]);
                }

                return new DefaultRabbitMqPersistentConnection(connectionFactory: factory, logger: logger,
                    retryCount: retryCount);
            });
            services.AddTransient<IConcurrencyRepository, RedisConcurrencyRepository>();

            this.RegisterEventBus(services: services);

            //create autofac based service provider
            var container = new ContainerBuilder();
            container.Populate(descriptors: services);


            return new AutofacServiceProvider(lifetimeScope: container.Build());
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
        }


        private void RegisterEventBus(IServiceCollection services)
        {
            var subscriptionClientName = this.Configuration[key: "SubscriptionClientName"];


            services.AddSingleton<IEventBus, EventBusRabbitMq>(implementationFactory: sp =>
            {
                var rabbitMqPersistentConnection = sp.GetRequiredService<IRabbitMqPersistentConnection>();
                var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
                var logger = sp.GetRequiredService<ILogger<EventBusRabbitMq>>();
                var eventBusSubscriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

                var retryCount = 5;
                if (!string.IsNullOrEmpty(value: this.Configuration[key: "EventBusRetryCount"]))
                {
                    retryCount = int.Parse(s: this.Configuration[key: "EventBusRetryCount"]);
                }

                return new EventBusRabbitMq(persistentConnection: rabbitMqPersistentConnection, logger: logger,
                    container: iLifetimeScope,
                    subsManager: eventBusSubscriptionsManager, queueName: subscriptionClientName,
                    retryCount: retryCount);
            });

            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
        }
    }
}