using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models.TronModels
{
    public class TransactionInfoModel
    {
        public long BlockNumber { get; set; }
        public long BlockTimeStamp { get; set; }
        public decimal Fee { get; set; }
        public List<string>? ContractResult { get; set; }
        public Receipt? Receipt { get; set; }
        public decimal PackingFee { get; set; }
    }
    public class Receipt
    {
        [JsonProperty("net_usage")]
        public long NetUsage { get; set; }
    }
}
