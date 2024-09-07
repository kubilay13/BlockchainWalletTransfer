using Microsoft.EntityFrameworkCore;
using System.Reflection;
using DataAccessLayer.AppDbContext;
using WalletsApi.Services;
using TronNet;
using Google.Api;
using ETHWalletApi.Services;
using Business.Services.EthWalletServices.EthTransferService;
using Business.BackgroundService.TronWalletBackgroundServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient(); // HttpClient ekleniyor
builder.Services.AddScoped<IWalletService, WalletServices>();
builder.Services.AddTransient<ITronService, TronService>();
builder.Services.AddTransient<IEthService, EthService>();
builder.Services.AddTransient<IEthTransferService, EthTransferService>();
builder.Services.AddScoped<ITronWalletService,TronWalletService>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApplicationDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"],
        s => s.MigrationsAssembly(typeof(Program).GetTypeInfo().Assembly.GetName().Name));
});
var tronNetOptions = builder.Configuration.GetSection("TronNet").Get<TronNetOptions>();
builder.Services.AddTronNet(x =>
{
    x.Network = TronNetwork.MainNet;
    x.Channel = new GrpcChannelOption { Host = "grpc.nile.trongrid.io", Port = 50051 };
    x.SolidityChannel = new GrpcChannelOption { Host = "grpc.nile.trongrid.io", Port = 50052 };
    x.ApiKey = "bbf6d1c9-daf4-49d9-a088-df29f664bac9";
});
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
