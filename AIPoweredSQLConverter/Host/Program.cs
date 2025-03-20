using Microsoft.OpenApi.Models;
using AIPoweredSQLConverter.API.Config;
using IntelligenceHub.Host.Config;
using AIPoweredSQLConverter.Common;
using AIPoweredSQLConverter.Host.Config;
using AIPoweredSQLConverter.Client.IntelligenceHub;
using AIPoweredSQLConverter.Business;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using System.Text.Json.Serialization;
using System.Security.Claims;
using AIPoweredSQLConverter.Host.Auth;
using Microsoft.AspNetCore.Authorization;
using AIPoweredSQLConverter.DAL; // Required for JsonStringEnumConverter
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AIPoweredSQLConverter.Host
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionStrings = builder.Configuration.GetRequiredSection(nameof(ConnectionStrings)).Get<ConnectionStrings>();
            var authSettings = builder.Configuration.GetRequiredSection(nameof(AuthSettings)).Get<AuthSettings>();
            var intelligenceHubSettings = builder.Configuration.GetRequiredSection(nameof(IntelligenceHubAuthSettings)).Get<IntelligenceHubAuthSettings>();

            // Ensure all API app settings exist
            if (string.IsNullOrEmpty(connectionStrings?.DbConnectionString) || string.IsNullOrEmpty(connectionStrings?.AppInsightsConnectionString))
            {
                throw new ArgumentException("Connection strings cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(authSettings?.BasicAuthUsername) || string.IsNullOrEmpty(authSettings?.BasicAuthPassword) ||
                string.IsNullOrEmpty(authSettings?.Audience) || string.IsNullOrEmpty(authSettings?.Domain))
            {
                throw new ArgumentException("Auth settings cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(intelligenceHubSettings?.Endpoint) || string.IsNullOrEmpty(intelligenceHubSettings?.AdminClientId) || 
                string.IsNullOrEmpty(intelligenceHubSettings?.AdminClientSecret) || string.IsNullOrEmpty(intelligenceHubSettings?.DefaultClientId) || 
                string.IsNullOrEmpty(intelligenceHubSettings?.DefaultClientSecret) || string.IsNullOrEmpty(intelligenceHubSettings?.Audience))
            {
                throw new ArgumentException("Intelligence Hub Auth settings cannot be null or empty.");
            }

            builder.Services.AddSingleton(connectionStrings);
            builder.Services.AddSingleton(authSettings);
            builder.Services.AddSingleton(intelligenceHubSettings);

            builder.Services.AddHttpClient();

            builder.Services.AddSingleton<AIAuthClient>();
            builder.Services.AddSingleton<IAIAuthClient, AIAuthClient>();
            builder.Services.AddSingleton<IAIClientWrapper, AIClientWrapper>();

            builder.Services.AddScoped<IPromptFlowLogic, PromptFlowLogic>();
            builder.Services.AddScoped<IAuthLogic, AuthLogic>();

            #region Authentication

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.Authority = $"https://{authSettings.Domain}/";
                options.Audience = authSettings.Audience;
                options.TokenValidationParameters = new TokenValidationParameters { NameClaimType = ClaimTypes.NameIdentifier, };
            });

            builder.Services.AddAuthorization(options =>
              {
                  options.AddPolicy("user:all", policy => policy.Requirements.Add(new HasScopeRequirement("user:all", $"https://{authSettings.Domain}/")));
              });

            builder.Services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();
            #endregion

            // Add EF Core DbContext
            builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionStrings.DbConnectionString));

            // Updated: Add controllers with JSON serialization settings that convert strings to enums
            builder.Services.AddControllersWithViews().AddJsonOptions(options => { options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); });

            // Register Swagger generator
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Conversational AI Website", Version = "v1" });
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' followed by a space and the JWT token."
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
                        Array.Empty<string>()
                    }
                });
            });

            // Add Logging via Application Insights
            builder.Services.AddApplicationInsightsTelemetry(options => { options.ConnectionString = connectionStrings?.AppInsightsConnectionString; });

            builder.Services.AddLogging(options =>
            {
                options.AddApplicationInsights();
            });

            // Program.cs
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigins", builder =>
                    builder
                        .WithOrigins("https://localhost:44483", "https://localhost:7228", "https://dev-64itzp4npb4uo8uv.us.auth0.com/*") // Front-end URL
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });

            var app = builder.Build();

            // Enable CORS
            app.UseCors("AllowSpecificOrigins");

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
