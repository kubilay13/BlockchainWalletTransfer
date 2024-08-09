using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services.TronWalletServices.BlockService
{
    internal interface IWalletBlockSerivce
    {
        Task GetReadBlock();
    }
}
