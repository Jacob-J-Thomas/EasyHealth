using Microsoft.OpenApi.Models;
using AiPocWebsiteTemplateWithBackend.API.Config;
using IntelligenceHub.Host.Config;
using Microsoft.AspNetCore.Authentication;
using AiPocWebsiteTemplateWithBackend.Host.Auth;
using AiPocWebsiteTemplateWithBackend.Common;
using AiPocWebsiteTemplateWithBackend.Host.Config;

namespace AiPocWebsiteTemplateWithBackend.Host
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var insightSettings = builder.Configuration.GetRequiredSection(nameof(AppInsightSettings)).Get<AppInsightSettings>();
            builder.Services.AddSingleton(builder.Configuration.GetRequiredSection(nameof(AIHubSettings)).Get<AIHubSettings>());
            builder.Services.AddSingleton(builder.Configuration.GetRequiredSection(nameof(AuthSettings)).Get<AuthSettings>());
            builder.Services.AddSingleton(builder.Configuration.GetRequiredSection(nameof(IntelligenceHubAuthSettings)).Get<IntelligenceHubAuthSettings>());

            builder.Services.AddHttpClient();

            builder.Services.AddAuthentication("BasicAuthentication")
                .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy(AuthPolicies.FunctionCallingAuthPolicy, policy => policy.RequireAuthenticatedUser());
            });

            builder.Services.AddControllersWithViews();

            // Register Swagger generator
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Conversational AI Website", Version = "v1" });

                // Add Basic Authentication
                c.AddSecurityDefinition("BasicAuthentication", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter your username and password",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "basic"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "BasicAuthentication"
                            }
                        },
                        new string[] {}
                    }
                });
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

            app.UseAuthentication();
            app.UseAuthorization();

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

