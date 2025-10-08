using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PropFinderApi.Contexts;
using PropFinderApi.Interfaces;
using PropFinderApi.Mappers;
using PropFinderApi.Middleware;
using PropFinderApi.Misc;
using PropFinderApi.Models;
using PropFinderApi.Repositories;
using PropFinderApi.Seed;
using PropFinderApi.Services;
using Serilog;

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Serilog Configuration
    builder.Host.UseSerilog((context, services, configuration) =>
    {
        configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext();
    });
    builder.Logging.ClearProviders();

    // Swagger Configuration
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "bearer"
        });
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
    builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

    // API Versioning
    builder.Services.AddApiVersioning(options =>
    {
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.ReportApiVersions = true;
        options.ApiVersionReader = new UrlSegmentApiVersionReader();
    });
    builder.Services.AddVersionedApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });

    // Controllers
    builder.Services.AddControllers()
        .AddJsonOptions(opts =>
        {
            opts.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            opts.JsonSerializerOptions.WriteIndented = true;
            opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

    // Rate Limiting
    builder.Services.AddRateLimiter(options =>
    {
        options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
        options.OnRejected = async (context, token) =>
        {
            context.HttpContext.Response.ContentType = "application/json";
            var response = new
            {
                success = false,
                message = "Too many requests. Please try again later.",
                data = (object)null,
                errors = new Dictionary<string, string[]>
                {
                    { "rateLimit", new[] { "You have exceeded the allowed number of requests. Try again shortly." } }
                }
            };
            var json = JsonSerializer.Serialize(response);
            await context.HttpContext.Response.WriteAsync(json, token);
        };

        options.AddPolicy("PerUserLimiter", context =>
        {
            var userId = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "anonymous";

            return RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: userId,
                factory: _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 500,
                    Window = TimeSpan.FromHours(1),
                    QueueLimit = 20,
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                });
        });
    });

    // Database Context
    builder.Services.AddDbContext<PropFinderDbContext>(opts =>
    {
        opts.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
    });
    builder.Services.Configure<AdminUserOptions>(builder.Configuration.GetSection("AdminUser"));

    // Repositories
     #region Repositories
    builder.Services.AddTransient<IRepository<Guid, User>, UserRepository>();
    builder.Services.AddTransient<IUserRepository, UserRepository>();
    builder.Services.AddTransient<IAgentProfileRepository, AgentProfileRepository>();
    builder.Services.AddTransient<IPropertyRepository, PropertyRepository>();
    builder.Services.AddTransient<IRepository<Guid, Property>, PropertyRepository>();
    builder.Services.AddTransient<IRepository<Guid, PropertyImage>, PropertyImageRepository>();
    builder.Services.AddTransient<IContactLogRepository, ContactLogRepository>();
    builder.Services.AddTransient<IRefreshTokenRepository, RefreshTokenRepository>();
    #endregion

    // Services
    #region Services
    builder.Services.AddTransient<IUserService, UserService>();
    builder.Services.AddTransient<IAuthenticationService, AuthenticationService>();
    builder.Services.AddTransient<IEncryptionService, EncryptionService>();
    builder.Services.AddTransient<ITokenService, TokenService>();
    builder.Services.AddTransient<IRefreshTokenService, RefreshTokenService>();
    builder.Services.AddTransient<IPropertyService, PropertyService>();
    builder.Services.AddTransient<IPropertyImageService, PropertyImageService>();
    builder.Services.AddTransient<IAgentProfileService, AgentProfileService>();
    builder.Services.AddTransient<IPaginationService, PaginationService>();
    builder.Services.AddTransient<IContactLogService, ContactService>();
    #endregion

    // Misc
    builder.Services.AddTransient<IApiResponseMapper, ApiResponseMapper>();

    // CORS
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy.WithOrigins("http://127.0.0.1:5500")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
    });

    // SignalR
    builder.Services.AddSignalR();

    // JWT Authentication & Authorization
    #region Authentication
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(builder.Configuration["Keys:JwtTokenKey"]))
            };
            options.Events = new JwtBearerEvents
            {
                OnChallenge = context =>
                {
                    var userId = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Anonymous";
                    var email = context.HttpContext.User.FindFirst(ClaimTypes.Email)?.Value ?? "Unknown";

                    Log.Warning("401 Unauthorized access attempt by {UserId} ({Email}) to {Path}", userId, email, context.Request.Path);

                    context.HandleResponse();
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";

                    var response = new
                    {
                        success = false,
                        message = "Unauthorized",
                        data = (object)null,
                        errors = new Dictionary<string, string[]>
                        {
                            { "general", new[] { "Authentication failed or token is missing/invalid." } }
                        }
                    };

                    var json = JsonSerializer.Serialize(response);
                    return context.Response.WriteAsync(json);
                },
                OnForbidden = context =>
                {
                    var userId = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Anonymous";
                    var email = context.HttpContext.User.FindFirst(ClaimTypes.Email)?.Value ?? "Unknown";

                    Log.Warning("403 Forbidden: {UserId} ({Email}) attempted to access {Path} without sufficient permissions.", userId, email, context.Request.Path);

                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    context.Response.ContentType = "application/json";

                    var response = new
                    {
                        success = false,
                        message = "Forbidden",
                        data = (object)null,
                        errors = new Dictionary<string, string[]>
                        {
                            { "general", new[] { "You do not have permission to access this." } }
                        }
                    };

                    var json = JsonSerializer.Serialize(response);
                    return context.Response.WriteAsync(json);
                }
            };
        });
    builder.Services.AddAuthorization();
    #endregion

    var app = builder.Build();

    // Swagger UI
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
            foreach (var description in provider.ApiVersionDescriptions)
            {
                options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                                        $"PropFinder API {description.GroupName.ToUpper()}");
            }
        });
    }

    // Request pipeline
    app.UseStaticFiles();
    app.UseCors();
    app.UseSerilogRequestLogging(options =>
    {
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("UserId", httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Anonymous");
            diagnosticContext.Set("UserEmail", httpContext.User.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty);
        };
    });
    app.UseMiddleware<ExceptionHandlingMiddleware>();
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseRateLimiter();
    app.MapHub<NotificationHub>("/notificationHub");
    app.MapControllers();

    // Seed database
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        await DbSeeder.SeedAdminAsync(services);
    }

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly.");
}
finally
{
    Log.CloseAndFlush();
}