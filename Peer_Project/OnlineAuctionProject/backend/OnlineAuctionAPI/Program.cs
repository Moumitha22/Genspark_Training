using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OnlineAuctionAPI.Contexts;
using OnlineAuctionAPI.Exceptions;
using OnlineAuctionAPI.Hubs;
using OnlineAuctionAPI.Interfaces;
using OnlineAuctionAPI.Interfaces.Repository;
using OnlineAuctionAPI.Mapping;
using OnlineAuctionAPI.Models;
using OnlineAuctionAPI.Repositories;
using OnlineAuctionAPI.Service;
using OnlineAuctionAPI.Services;
using QuestPDF.Infrastructure;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

#region appsettings.json config
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false)
    .Build();

var keyVaultUrl = configuration["AzureKeyVault:myVaultUrl"];
SecretClient? secretClient = null;
try
{
    if (!string.IsNullOrEmpty(keyVaultUrl))
    {
        secretClient = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());
        builder.Services.AddSingleton(secretClient);
    }
    else
    {
        Console.WriteLine("Warning: Azure KeyVault URL is missing from configuration.");
        builder.Services.AddSingleton<SecretClient>(_ => null);
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Warning: Failed to create SecretClient: {ex.Message}");
    builder.Services.AddSingleton<SecretClient>(_ => null);
}
#endregion

#region QuestPdf license
QuestPDF.Settings.License = LicenseType.Community;
#endregion

#region Keyvault
// KeyVaultSecret accessConnection = await secretClient.GetSecretAsync("AccessConnectionUrl");
// var accessConnectionString = accessConnection.Value;
#endregion

// #region logger
// var logger = new LoggerConfiguration()
//     .MinimumLevel.Information()
//     .Enrich.FromLogContext()
//     .WriteTo.Console(
//         outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
//     )
// .WriteTo.AzureBlobStorage(
//     connectionString: accessConnectionString,
//     storageContainerName: "backend-logs",
//     storageFileName: "log.txt",
//     outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
//     restrictedToMinimumLevel: LogEventLevel.Information
// )
//     .CreateLogger();
// #endregion

builder.Logging.ClearProviders();
// builder.Logging.AddSerilog(logger);

#region logger
builder.Host.UseSerilog(
    (context, services, configuration) =>
        configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
);
#endregion

builder.Services.AddEndpointsApiExplorer();

#region Swagger
builder.Services.AddSwaggerGen(opt =>
{
    opt.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "bearer",
        }
    );
    opt.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer",
                    },
                },
                new string[] { }
            },
        }
    );
});
#endregion

builder.Services.AddDbContext<AuctionContext>(opts =>
{
    opts.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// #region Database Connection
// KeyVaultSecret dbConnection = await secretClient.GetSecretAsync("DBConnectionString");
// var dbConnectionString = dbConnection.Value;

// builder.Services.AddDbContext<AuctionContext>(opts =>
// {
//     opts.UseNpgsql(dbConnectionString);
// });
// #endregion

builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

builder.Services.AddAutoMapper(typeof(AuctionItemProfile));
builder.Services.AddAutoMapper(typeof(UserProfile));
builder.Services.AddAutoMapper(typeof(BidProfile));

#region Controllers
builder
    .Services.AddControllers()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.ReferenceHandler = System
            .Text
            .Json
            .Serialization
            .ReferenceHandler
            .Preserve;
        opts.JsonSerializerOptions.WriteIndented = true;
        opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddControllers(options =>
{
    options.Filters.Add<CustomValidationFilter>();
});
#endregion

#region  API Versioning
builder.Services.AddApiVersioning(opt =>
{
    opt.DefaultApiVersion = new ApiVersion(1, 0);
    opt.AssumeDefaultVersionWhenUnspecified = true;
    opt.ReportApiVersions = true;
});
builder.Services.AddVersionedApiExplorer(opt =>
{
    opt.GroupNameFormat = "'v'VVV";
    opt.SubstituteApiVersionInUrl = true;
});
#endregion

#region AuthenticationFilter
builder
    .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Keys:JwtTokenKey"])
            ),
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];

                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/auctionhub"))
                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            },

            OnChallenge = context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                var result = System.Text.Json.JsonSerializer.Serialize(
                    new
                    {
                        success = false,
                        message = "Unauthorized",
                        data = (object)null,
                        errors = (object)null,
                    }
                );
                return context.Response.WriteAsync(result);
            },
            OnForbidden = context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/json";
                var result = System.Text.Json.JsonSerializer.Serialize(
                    new
                    {
                        success = false,
                        message = "Forbidden",
                        data = (object)null,
                        errors = (object)null,
                    }
                );
                return context.Response.WriteAsync(result);
            },
        };
    });
#endregion

#region Repositories
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<IAuctionItemRepository, AuctionRepository>();
builder.Services.AddTransient<IBidItemRepository, BidItemRepository>();
builder.Services.AddTransient<AuctionRepository>();
builder.Services.AddTransient<IEAgreementRepository, EAgreementRepository>();
builder.Services.AddTransient<IVirtualWalletRepository, VirtualWalletRepository>();
builder.Services.AddTransient<IUserAccountManageRepository, UserAccountManageRepository>();
builder.Services.AddTransient<IAuctionDeleteRequestRepository, AuctionDeleteRequestRepository>();

#endregion

#region Services
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IPasswordService, PasswordService>();
builder.Services.AddTransient<IAuctionItemService, AuctionItemService>();
builder.Services.AddTransient<ITokenService, TokenService>();
builder.Services.AddTransient<IAuthService, OnlineAuctionAPI.Service.AuthenticationService>();
builder.Services.AddTransient<IBidItemService, BidItemService>();
builder.Services.AddHostedService<AuctionCheckService>();
builder.Services.AddTransient<IEAgreementService, EAgreementService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddTransient<IVirtualWalletService, VirtualWalletService>();
builder.Services.AddTransient<IUserAccountManageService, UserAccountManageService>();
builder.Services.AddTransient<IAuctionDeleteRequestService, AuctionDeleteRequestService>();
#endregion

#region CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .WithOrigins("http://127.0.0.1:5500", "http://127.0.0.1:5501", "http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});
#endregion

builder.Services.AddSignalR();

#region Rate Limiting
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
    {
        var user = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "anonymous";
        return RateLimitPartition.GetTokenBucketLimiter(
            user,
            _ =>
            {
                return new TokenBucketRateLimiterOptions
                {
                    TokenLimit = 100,
                    TokensPerPeriod = 1000,
                    ReplenishmentPeriod = TimeSpan.FromHours(1),
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = 0,
                };
            }
        );
    });

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});
#endregion


// Program.cs
builder.Host.ConfigureHostOptions(opt =>
{
    opt.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
});

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

// app.UseHttpsRedirection();
app.UseRateLimiter();
app.MapControllers();
app.MapHub<AuctionHub>("/auctionHub");

app.UseMiddleware<ExceptionMiddleware>();

app.Run();


// Case Study 13: Online Auction API
// Roles: Seller, Bidder
// Features:
// Post items for auction
// Place live bids
// SignalR for bid updates in real-time"
