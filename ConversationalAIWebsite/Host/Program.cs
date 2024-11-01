using Microsoft.OpenApi.Models;
using AiPocWebsiteTemplateWithBackend.API.Config;
using IntelligenceHub.Host.Config;

namespace AiPocWebsiteTemplateWithBackend.Host
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var insightSettings = builder.Configuration.GetRequiredSection(nameof(AppInsightSettings)).Get<AppInsightSettings>();

            builder.Services.AddControllersWithViews();
            builder.Services.AddSingleton(builder.Configuration.GetRequiredSection(nameof(AIHubSettings)).Get<AIHubSettings>());
            builder.Services.AddSingleton(builder.Configuration.GetRequiredSection(nameof(AuthSettings)).Get<AuthSettings>());
            builder.Services.AddHttpClient();

            // Add SignalR
            builder.Services.AddSignalR();

            // Register Swagger generator
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Conversational AI Website", Version = "v1" });
            });

            // Add Logging via Application Insights
            builder.Services.AddApplicationInsightsTelemetry(options =>
            {
                options.ConnectionString = insightSettings?.ConnectionString;
            });

            builder.Services.AddLogging(options =>
            {
                options.AddApplicationInsights();
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });

            var app = builder.Build();

            // Enable CORS
            app.UseCors("AllowAllOrigins");

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            // Enable Swagger UI
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API V1");
                c.RoutePrefix = "swagger"; // Set Swagger UI path
            });

            // Map controllers and default route
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller}/{action=Index}/{id?}");

            // Map fallback route to serve index.html
            app.MapFallbackToFile("index.html");

            app.Run();

        }
    }
}

