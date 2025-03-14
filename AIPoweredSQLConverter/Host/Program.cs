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

namespace AIPoweredSQLConverter.Host
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

            builder.Services.AddSingleton<AIAuthClient>();
            builder.Services.AddSingleton<IAIAuthClient, AIAuthClient>();
            builder.Services.AddSingleton<IAIClientWrapper, AIClientWrapper>();

            builder.Services.AddScoped<IPromptFlowLogic, PromptFlowLogic>();
            builder.Services.AddScoped<IAuthLogic, AuthLogic>();

            #region Authentication

            // Configure Auth
            var authSettings = builder.Configuration.GetRequiredSection(nameof(AuthSettings)).Get<AuthSettings>();
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.Authority = authSettings.Domain;
                options.Audience = authSettings.Audience;

                // Specify the Role Claim Type if necessary
                //options.TokenValidationParameters = new TokenValidationParameters
                //{
                //    RoleClaimType = "roles"
                //};
            });

            // Add role-based authorization policies
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy(AuthPolicies.PayingUserAuthPolicy, policy =>
                    policy.RequireAssertion(context =>
                        context.User.HasClaim(c => (c.Type == "scope" || c.Type == "permissions") && c.Value.Split(' ').Contains("all:elevated"))));
            });
            #endregion

            builder.Services.AddControllersWithViews();

            // Register Swagger generator
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Conversational AI Website", Version = "v1" });

                // Define the security scheme for bearer tokens
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' followed by a space and the JWT token."
                });

                // Apply the security scheme globally
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
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

