using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class ServiceProcessingResult
    {
        public bool IsSuccessful { get; set; }
        public ProcessingError Error { get; set; }

        public bool IsFatalFailure()
        {
            return !IsSuccessful && Error.IsFatal;
        }
    }

    public class ServiceProcessingResult<TReturnedData> : ServiceProcessingResult
    {
        public TReturnedData Data { get; set; }
    }
}
