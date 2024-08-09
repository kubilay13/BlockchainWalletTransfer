using ETHWalletApi.Services;

var builder = WebApplication.CreateBuilder(args);

// `EthService` s�n�f�n� ve ba��ml�l�klar� yap�land�r�n
var nodeUrl = "https://sepolia.infura.io/v3/3fcb68529b9e4288a4eb599f266bbb50"; // Sepolia a�� i�in uygun URL
builder.Services.AddScoped<IEthService>(sp => new EthService(nodeUrl));
// `EthService`'i URL ile olu�turun

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
