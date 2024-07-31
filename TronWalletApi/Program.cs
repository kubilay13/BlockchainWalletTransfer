using Microsoft.EntityFrameworkCore;
using System.Reflection;
using TronNet;
using TronWalletApi.Context;
using TronWalletApi.Models;
using TronWalletApi.BackgroundServices;
using TronWalletApi.Services.TronWalletService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApplicationDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"],
        s => s.MigrationsAssembly(typeof(Program).GetTypeInfo().Assembly.GetName().Name));
});

builder.Services.AddScoped<ITronWalletService, TronWalletService>();

// Configure other services
var tronNetOptions = builder.Configuration.GetSection("TronNet").Get<TronNetOptions>();
builder.Services.AddTronNet(x =>
{
    x.Network = TronNetwork.MainNet;
    x.Channel = new GrpcChannelOption { Host = "grpc.nile.trongrid.io", Port = 50051 };
    x.SolidityChannel = new GrpcChannelOption { Host = "grpc.nile.trongrid.io", Port = 50052 };
    x.ApiKey = "bbf6d1c9-daf4-49d9-a088-df29f664bac9";
});
builder.Services.AddHttpClient();
builder.Services.AddScoped<ITronService, TronService>();
builder.Services.AddHostedService<TronWalletAmountUpdateService>(); // Add hosted service

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
