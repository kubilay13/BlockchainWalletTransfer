using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using TronWalletApi.Services.TronWalletService;

namespace TronWalletApi.BackgroundServices
{
    public class TronWalletAmountUpdateService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<TronWalletAmountUpdateService> _logger;

        public TronWalletAmountUpdateService(IServiceProvider serviceProvider, ILogger<TronWalletAmountUpdateService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("TronWalletAmountUpdateService starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("TronWalletAmountUpdateService running.");

                using (var scope = _serviceProvider.CreateScope())
                {
                    var tronWalletService = scope.ServiceProvider.GetRequiredService<ITronWalletService>();

                    try
                    {

                        await tronWalletService.UpdateWalletAmountsAsync();
                        //await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                        await tronWalletService.GetNetworkFee();
                        _logger.LogInformation("Cüzdan bakiyeleri güncellendi 10 saniye sonra tekrarlanacak.");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "An error occurred while updating wallet amounts or network fees.");
                    }
                }

                await Task.Delay(TimeSpan.FromMilliseconds(5000), stoppingToken);
            }

            _logger.LogInformation("TronWalletAmountUpdateService stopping.");
        }
    }
}
