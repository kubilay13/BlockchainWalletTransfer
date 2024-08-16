using DataAccessLayer.AppDbContext;
using ETHWalletApi.Services;
using Google.Api;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

var nodeUrl = "https://sepolia.infura.io/v3/3fcb68529b9e4288a4eb599f266bbb50"; 
var privateKey = "0107932b30922231adff71b4b7c0b05bc948632f56c2b62f98bd18fefeae8a9e"; 

builder.Services.AddScoped<IEthService, EthService>();
builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApplicationDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"],
        s => s.MigrationsAssembly(typeof(Program).GetTypeInfo().Assembly.GetName().Name));
});
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
