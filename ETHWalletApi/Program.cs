using ETHWalletApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Ethereum d���m URL'si ve �zel anahtar
var nodeUrl = "https://sepolia.infura.io/v3/3fcb68529b9e4288a4eb599f266bbb50"; // Sepolia a�� i�in uygun URL
var privateKey = "0107932b30922231adff71b4b7c0b05bc948632f56c2b62f98bd18fefeae8a9e"; // Ger�ek �zel anahtar�n�z� buraya yaz�n

// `EthService` s�n�f�n� ve ba��ml�l�klar� yap�land�r�n
builder.Services.AddScoped<IEthService>(sp => new EthService(nodeUrl, privateKey));

builder.Services.AddControllers();
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
