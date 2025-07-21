using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using TrainingVideoPortalAPI.Contexts;
using TrainingVideoPortalAPI.Interfaces;
using TrainingVideoPortalAPI.Models;
using TrainingVideoPortalAPI.Repositories;
using TrainingVideoPortalAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Controllers
builder.Services.AddControllers()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        opts.JsonSerializerOptions.WriteIndented = true;
    });

builder.Services.AddDbContext<TrainingVideoDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddTransient<IRepository<Guid, TrainingVideo>, TrainingVideoRepository>();
builder.Services.AddTransient<ITrainingVideoService, TrainingVideoService>();
builder.Services.AddTransient<BlobStorageService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost4200", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowLocalhost4200");
app.MapControllers();

app.Run();


