using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiBindingModels {
    public class UploadFileResponseBindingModel {
        public bool IsError { get; set; }
        public string DocumentBlobUri { get; set; }
    }

    public class ConsumerDataObject {
        public string UserAgent { get; set; }
        public string IP { get; set; }
        public string TransactionID { get; set; }
    }

    public class Base64UploadRequestBindingModel {
        public string base64File { get; set; }
    }
}
