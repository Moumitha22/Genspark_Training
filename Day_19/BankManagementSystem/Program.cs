using BankManagementSystem.Models;
using BankManagementSystem.Interfaces;
using BankManagementSystem.Contexts;
using BankManagementSystem.Repositories;
using Microsoft.EntityFrameworkCore;
using BankManagementSystem.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers()
        .AddJsonOptions(opts =>
        {
            opts.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
            opts.JsonSerializerOptions.WriteIndented = true;
        });

builder.Services.AddDbContext<BankContext>(opts =>
{
    opts.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});


builder.Services.AddTransient<IRepository<int, User>, UserRepository>();
builder.Services.AddTransient<IRepository<int, Account>, AccountRepository>();
builder.Services.AddTransient<IRepository<int, Transaction>, TransactionRepository>();

builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IAccountService, AccountService>();
builder.Services.AddTransient<ITransactionService, TransactionService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();


