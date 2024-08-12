
using ETHWalletApi.Models;
using Microsoft.EntityFrameworkCore;
namespace ETHWalletApi.AppDbContext
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext>options):base(options) 
        { 
        
        }
        public DbSet<EthWalletModels> ETHWalletModels { get; set; }
        public DbSet<EthNetworkTransactionRequest> ETHNetworkTransactionRequests { get; set; }
    }
}
