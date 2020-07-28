using DataModel;
using DataModel.Models.Message;
using Events;
using Infrastructure.Consul;
using Infrastructure.Core;
using Infrastructure.EventBus.RabbitMQ;
using Infrastructure.Logging;
using Infrastructure.Outbox;
using Infrastructure.Swagger;
using MessagesService.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Reflection;

namespace MessagesService
{
    public class Startup
    {
        private readonly IConfigurationRoot Configuration;

        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", reloadOnChange: true, optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();

            Log.Logger = LoggingExtensions.AddLogging(Configuration);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DatabaseContext>(options => options.UseSqlServer(Configuration.GetConnectionString(ConnectionStringKeys.App)));

            services.AddSingleton<IMessageRepository, MessageRepository>();

            services
                .AddConsul(Configuration)
                .AddRabbitMQ(Configuration)
                .AddOutbox(Configuration)
                .AddSwagger(Configuration)
                .AddCore(typeof(Startup).GetTypeInfo().Assembly);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory, IHostApplicationLifetime lifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app
                .UseLogging(Configuration, loggerFactory)
                .UseSwagger(Configuration)
                .UseConsul(lifetime)
                .UseCore();

            app.UseSubscribeAllEvents();
        }
    }
}
