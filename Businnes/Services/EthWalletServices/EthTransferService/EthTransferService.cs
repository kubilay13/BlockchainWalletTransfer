using Business.Services.WalletPrivatekeyToPasswords;
using DataAccessLayer.AppDbContext;
using Entities.Dto.WalletApiDto;
using Entities.Models.TronModels;
using ETHWalletApi.Services;
using Microsoft.EntityFrameworkCore;
using Nethereum.Contracts.Standards.ERC20.ContractDefinition;
using Nethereum.Hex.HexTypes;
using Nethereum.JsonRpc.Client;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Signer;
using Nethereum.Web3;

namespace Business.Services.EthWalletServices.EthTransferService
{

    public class EthTransferService:IEthTransferService
    {
        private readonly IWeb3 _web3;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IWalletPrivatekeyToPassword _walletPrivatekeyToPassword;

        public EthTransferService(IWeb3 web3, ApplicationDbContext applicationDbContext, IWalletPrivatekeyToPassword walletPrivatekeyToPassword)
        {
            _applicationDbContext = applicationDbContext;
            _web3 = web3;
            _walletPrivatekeyToPassword = walletPrivatekeyToPassword;
        }

        public async Task<string> SendTransactionAsyncETH(TransferRequest request)
        {
            var senderAddress = await _applicationDbContext.WalletDetailModels.FirstOrDefaultAsync(w => w.WalletAddressETH == request.SenderAddress);
            byte[] encryptedPrivateKeyBytes = Convert.FromBase64String(senderAddress.PrivateKeyEth);
            string decryptedPrivateKey = _walletPrivatekeyToPassword.DecryptPrivateKey(encryptedPrivateKeyBytes);

            var account = new Nethereum.Web3.Accounts.Account(decryptedPrivateKey);
            var web3 = new Web3(account, _web3.Client);

            var amountInWei = Web3.Convert.ToWei(request.Amount);
            var currentNonce = await web3.Eth.Transactions.GetTransactionCount.SendRequestAsync(account.Address);

            try
            {
                var transaction = new TransactionInput
                {
                    From = request.SenderAddress,
                    To = request.ReceiverAddress,
                    Value = new HexBigInteger(amountInWei),
                    Nonce = currentNonce,
                    GasPrice = await web3.Eth.GasPrice.SendRequestAsync()
                };

                var gasEstimate = await web3.Eth.Transactions.EstimateGas.SendRequestAsync(transaction);
                transaction.Gas = gasEstimate;

                var signature = await web3.TransactionManager.SignTransactionAsync(transaction);
                var txnHash = await web3.Eth.Transactions.SendRawTransaction.SendRequestAsync(signature);

                var EthTransaction = new TransferHistoryModel
                {
                    SendingAddress = request.SenderAddress,
                    ReceivedAddress = request.ReceiverAddress,
                    TransactionHash = txnHash,
                    CoinType = "Ethereum",
                    TransactionNetwork = "ETH",
                    TransactionAmount = request.Amount,
                    TransactionDate = DateTime.UtcNow,
                    Commission = 0,
                    NetworkFee = Convert.ToDecimal(gasEstimate.ToString()),
                    TransactionUrl = $"https://sepolia.etherscan.io/tx/{txnHash}",
                    TransactionStatus = true,
                    TransactionType = 0,
                    Network = "TestNet(Sepolia)"
                };

                _applicationDbContext.TransferHistoryModels.Add(EthTransaction);
                await _applicationDbContext.SaveChangesAsync();

                var transactionUrl = $"https://sepolia.etherscan.io/tx/{txnHash}";
                return "ETH Transfer İşleminiz Başarılı:  " + transactionUrl;
            }
            catch (RpcResponseException ex)
            {
                throw new InvalidOperationException($"ETH Transfer İşlemi Başarısız: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("ETH Transfer İşleminde Beklenmeyen Bir Hata Oluştu. ", ex);
            }
        }
        public async Task<string> SendTransactionAsyncUSDT(TransferRequest request)
        {
            var usdtcontractadress = "0x2DCe21ca7F38D7Fbb6Bbf86AC11ec7867A510f24";
            var senderAddress = await _applicationDbContext.WalletDetailModels.FirstOrDefaultAsync(w => w.WalletAddressETH == request.SenderAddress);

            byte[] encryptedPrivateKeyBytes = Convert.FromBase64String(senderAddress.PrivateKeyEth);
            string decryptedPrivateKey = _walletPrivatekeyToPassword.DecryptPrivateKey(encryptedPrivateKeyBytes);

            var account = new Nethereum.Web3.Accounts.Account(decryptedPrivateKey, Chain.Sepolia);

            var web3 = new Web3(account, "https://sepolia.infura.io/v3/3fcb68529b9e4288a4eb599f266bbb50");
            var amountInWei = Web3.Convert.ToWei(request.Amount, 6);
            var currentNonce = await web3.Eth.Transactions.GetTransactionCount.SendRequestAsync(account.Address);
            var gasPrice = await web3.Eth.GasPrice.SendRequestAsync();

            try
            {
                var transferHandler = web3.Eth.GetContractTransactionHandler<TransferFunction>();

                var transferFunction = new TransferFunction
                {
                    FromAddress = request.SenderAddress,
                    To = request.ReceiverAddress,
                    Value = amountInWei,
                    Nonce = currentNonce,
                    GasPrice = gasPrice,
                };

                var transferReceipt = await transferHandler.SendRequestAsync(usdtcontractadress, transferFunction);
                return transferReceipt;
            }
            catch (RpcResponseException ex)
            {
                throw new InvalidOperationException($"ETH Transfer İşlemi Başarısız: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("ETH Transfer İşleminde Beklenmeyen Bir Hata Oluştu.", ex);
            }
        }
        public async Task<string> SendTransactionAsyncBnb(TransferRequest request)
        {
            var BnbContractAdress = "0x17c3fD32E71b97Ae7EA1B5dCa135846461a8F6B6";
            var senderAddress = await _applicationDbContext.WalletDetailModels.FirstOrDefaultAsync(w => w.WalletAddressETH == request.SenderAddress);

            byte[] encryptedPrivateKeyBytes = Convert.FromBase64String(senderAddress.PrivateKeyEth);
            string decryptedPrivateKey = _walletPrivatekeyToPassword.DecryptPrivateKey(encryptedPrivateKeyBytes);

            var account = new Nethereum.Web3.Accounts.Account(decryptedPrivateKey, Chain.Sepolia);

            var web3 = new Web3(account, "https://sepolia.infura.io/v3/3fcb68529b9e4288a4eb599f266bbb50");
            var amountInWei = Web3.Convert.ToWei(request.Amount, 18);
            var currentNonce = await web3.Eth.Transactions.GetTransactionCount.SendRequestAsync(account.Address);
            var gasPrice = await web3.Eth.GasPrice.SendRequestAsync();

            try
            {
                var transferHandler = web3.Eth.GetContractTransactionHandler<TransferFunction>();

                var transferFunction = new TransferFunction
                {
                    FromAddress = request.SenderAddress,
                    To = request.ReceiverAddress,
                    Value = amountInWei,
                    Nonce = currentNonce,
                    GasPrice = gasPrice,
                };

                var transferReceipt = await transferHandler.SendRequestAsync(BnbContractAdress, transferFunction);
                return transferReceipt;
            }
            catch (RpcResponseException ex)
            {
                throw new InvalidOperationException($"ETH Transfer İşlemi Başarısız: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("ETH Transfer İşleminde Beklenmeyen Bir Hata Oluştu.", ex);
            }
        }

        public async Task<string>SendTransactionAsyncBnb_Bnb(TransferRequest request)
        {
            var senderAddress = await _applicationDbContext.WalletDetailModels
                .FirstOrDefaultAsync(w => w.WalletAddressETH == request.SenderAddress);

            byte[] encryptedPrivateKeyBytes = Convert.FromBase64String(senderAddress.PrivateKeyEth);
            string decryptedPrivateKey = _walletPrivatekeyToPassword.DecryptPrivateKey(encryptedPrivateKeyBytes);

            var account = new Nethereum.Web3.Accounts.Account(decryptedPrivateKey);

            var _web3 = new Web3(account, "https://data-seed-prebsc-1-s1.binance.org:8545/");
            var currentNonce = await _web3.Eth.Transactions.GetTransactionCount.SendRequestAsync(account.Address);
            var amountInWei = Web3.Convert.ToWei(request.Amount);
            var gasPrice = await _web3.Eth.GasPrice.SendRequestAsync();
            var transactionInput = new TransactionInput
            {
                From = request.SenderAddress,
                To = request.ReceiverAddress,
                Value = new HexBigInteger(amountInWei),
                Nonce = new HexBigInteger(currentNonce),
                GasPrice = gasPrice,
                Gas=new HexBigInteger(21000),
            };
            try
            {
                var txsignature= await _web3.TransactionManager.SignTransactionAsync(transactionInput);
                var txnHash = await _web3.Eth.Transactions.SendRawTransaction.SendRequestAsync(txsignature);
                return txnHash;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("BNB Testnet Transfer İşleminde Beklenmeyen Bir Hata Oluştu.", ex);
            }

        }
    }
}
