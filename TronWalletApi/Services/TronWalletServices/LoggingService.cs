using Serilog;
namespace TronWalletApi.Services.TronWalletServices
{
    public class LoggingService:ILoggingService
    {
        public void LogMessages()
        {
            Log.Information("İşlem başarılı");
            var coloredMessage = "\x1b{Timestamp:yyyy-MM-dd HH:mm:ss} Transfer Ücreti Güncellendi\x1b[0m";
            Log.Information(coloredMessage);
            Log.Information("Başarılı bir işlem yapıldı.");
            Log.Error("Bir hata oluştu.");
        }
    }
}
