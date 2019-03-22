using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiBindingModels {
    public class SalesForceBindingModel
    {
        public string OracleNumber { get; set; }
        public string MemorybookCode { get; set; }
        public string ContractYear { get; set; }
        public string ContractLoadDate { get; set; }
        public string Staging { get; set; }
        public string Publisher { get; set; }
        public string Pages { get; set; }
        public string Copies { get; set; }
        public string BookPrice { get; set; }
        public string SubTotal { get; set; }
        public string Status { get; set; }
        public string RowNumber { get; set; }
        public string CSR { get; set; }
        public string Shipdate { get; set; }
        public string InvoiceNumber { get; set; }
        public bool AgreementReceived { get; set; }
        public string AgreementRecDate { get; set; }
        public string KitReceivedDate { get; set; }
        public string DeadLineDayIn{get;set;}
        public string OnlinePayCloseOutDate { get; set; }
        public string JobNo { get; set; }
        public string SpecCvrRecDate { get; set; }
        public string CoverType { get; set; }

    }

    
}
