namespace AUTOPAL.RTL.TVMaze.Services.TVShows.API
{
    using System;
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using AUTOPAL.RTL.TVMaze.BuildingBlocks.EventBus.Common.Abstractions;
    using AUTOPAL.RTL.TVMaze.Services.TVShows.API.Infrastructure;
    using AUTOPAL.RTL.TVMaze.Services.TVShows.API.IntegrationEvents.EventHandling;
    using AUTOPAL.RTL.TVMaze.Services.TVShows.API.IntegrationEvents.Events;
    using AUTOPAL.RTL.TVMaze.Services.TVShows.API.Model;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

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
            services.AddCustomMvc(configuration: this.Configuration)
                .AddCustomDbContext(configuration: this.Configuration)
                .AddCustomOptions(configuration: this.Configuration)
                .AddIntegrationServices(configuration: this.Configuration)
                .AddEventBus(configuration: this.Configuration)
                .AddSwagger();

            services.AddTransient<DbContext, TVShowContext>();
            services.AddTransient<IGenericRepository<Show>, GenericRepository<Show>>();

            var container = new ContainerBuilder();
            container.Populate(descriptors: services);
            return new AutofacServiceProvider(lifetimeScope: container.Build());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            var pathBase = this.Configuration[key: "PATH_BASE"];

            if (!string.IsNullOrEmpty(value: pathBase))
            {
                app.UsePathBase(pathBase: pathBase);
            }

            app.UseCors(policyName: "CorsPolicy");

            app.UseMvcWithDefaultRoute();

            app.UseSwagger()
                .UseSwaggerUI(setupAction: c =>
                {
                    c.SwaggerEndpoint(
                        url:
                        $"{(!string.IsNullOrEmpty(value: pathBase) ? pathBase : string.Empty)}/swagger/v1/swagger.json",
                        name: "TvShows.API V1");
                });


            this.ConfigureEventBus(app: app);
        }

        protected virtual void ConfigureEventBus(IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
            eventBus.Subscribe<ShowUpdatedIntegrationEvent, ShowUpdatedIntegrationEventHandler>();
        }
    }
}