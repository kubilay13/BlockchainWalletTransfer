using Microsoft.EntityFrameworkCore;
using System.Reflection;
using TronNet;
using TronWalletApi.BackgroundServices;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using DataAccessLayer.AppDbContext;
using Business.Services.WalletPrivatekeyToPasswords;
using Business.BackgroundService.TronWalletBackgroundServices;
using Business.Services.TronWalletService.CreateWalletTron;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSerilog();
builder.Host.UseSerilog((context, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration)
                 .WriteTo.Console(theme: AnsiConsoleTheme.Code, outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}");
});

builder.Services.AddHttpClient();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApplicationDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"],
        s => s.MigrationsAssembly(typeof(Program).GetTypeInfo().Assembly.GetName().Name));
});
builder.Services.AddTransient<ITronWalletService, TronWalletService>();
builder.Services.AddTransient<IWalletPrivatekeyToPassword, WalletPrivatekeyToPassword>();
builder.Services.AddScoped<ITronWalletService, TronWalletService>();
builder.Services.AddScoped<ITronService, TronService>();
builder.Services.AddScoped<ICreateWalletTron,CreateWalletTron>();
var tronNetOptions = builder.Configuration.GetSection("TronNet").Get<TronNetOptions>();
builder.Services.AddTronNet(x =>
{
    x.Network = TronNetwork.MainNet;
    x.Channel = new GrpcChannelOption { Host = "grpc.nile.trongrid.io", Port = 50051 };
    x.SolidityChannel = new GrpcChannelOption { Host = "grpc.nile.trongrid.io", Port = 50052 };
    x.ApiKey = "bbf6d1c9-daf4-49d9-a088-df29f664bac9";
});
builder.Services.AddHttpClient();

builder.Services.AddHostedService<TronWalletAmountUpdateService>();


var app = builder.Build();
app.UseSerilogRequestLogging();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
