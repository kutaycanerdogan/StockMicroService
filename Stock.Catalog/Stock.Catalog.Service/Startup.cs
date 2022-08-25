using System;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Polly;
using Polly.Timeout;
using Stock.Catalog.Service.Entities;
using Stock.Common.MassTransit;
using Stock.Common.MongoDB;
using Stock.Common.Settings;
using Swashbuckle.AspNetCore;

namespace Stock.Catalog.Service
{
    public class Startup
    {
        private const string AllowedOriginSetting = "AllowedOrigin";
        private ServiceSettings serviceSettings;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            serviceSettings = Configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
            services.AddMongo()
                    .AddMongoRepository<Product>("Products")
                    .AddMongoRepository<Variant>("Variants")
                    .AddMassTransitWithRabbitMq();

            // AddCatalogClient(services);

            services.AddControllers(options =>
             {
                 options.SuppressAsyncSuffixInActionNames = false;
             });
            services.AddCors();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "1.0.0",
                    Title = "API Swagger",
                    Description = "Api Swagger Documentation",

                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Stock.Catalog.Service v1");
                    c.RoutePrefix = string.Empty;
                });
                app.UseCors(builder =>
                                    {
                                        builder.WithOrigins(Configuration[AllowedOriginSetting])
                                            .AllowAnyHeader()
                                            .AllowAnyMethod();
                                    });
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
                endpoints.MapControllers();
            });
        }

        // private static void AddCatalogClient(IServiceCollection services)
        // {
        //     Random jitterer = new Random();

        //     services.AddHttpClient<CatalogClient>(client =>
        //     {
        //         client.BaseAddress = new Uri("https://localhost:5001");
        //     })
        //     .AddTransientHttpErrorPolicy(builder => builder.Or<TimeoutRejectedException>().WaitAndRetryAsync(
        //         5,
        //         retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
        //                         + TimeSpan.FromMilliseconds(jitterer.Next(0, 1000)),
        //         onRetry: (outcome, timespan, retryAttempt) =>
        //         {
        //             var serviceProvider = services.BuildServiceProvider();
        //             serviceProvider.GetService<ILogger<CatalogClient>>()?
        //                 .LogWarning($"Delaying for {timespan.TotalSeconds} seconds, then making retry {retryAttempt}");
        //         }
        //     ))
        //     .AddTransientHttpErrorPolicy(builder => builder.Or<TimeoutRejectedException>().CircuitBreakerAsync(
        //         3,
        //         TimeSpan.FromSeconds(15),
        //         onBreak: (outcome, timespan) =>
        //         {
        //             var serviceProvider = services.BuildServiceProvider();
        //             serviceProvider.GetService<ILogger<CatalogClient>>()?
        //                 .LogWarning($"Opening the circuit for {timespan.TotalSeconds} seconds...");
        //         },
        //         onReset: () =>
        //         {
        //             var serviceProvider = services.BuildServiceProvider();
        //             serviceProvider.GetService<ILogger<CatalogClient>>()?
        //                 .LogWarning($"Closing the circuit...");
        //         }
        //     ))
        //     .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(1));
        // }
    }
}