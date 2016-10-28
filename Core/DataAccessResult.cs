using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class DataAccessResult
    {
        public bool IsSuccessful { get; set; }

        public ProcessingError Error { get; set; }

        public virtual ServiceProcessingResult ToServiceProcessingResult(ProcessingError processingError)
        {
            var processingResult = new ServiceProcessingResult
            {
                IsSuccessful = IsSuccessful
            };
            if (!IsSuccessful)
            {
                processingResult.Error = Error.CanBeFixedByUser ? Error : processingError;
            }
            return processingResult;
        }
    }

    public class DataAccessResult<TReturnedData> : DataAccessResult
    {
        public TReturnedData Data { get; set; }

        public ServiceProcessingResult<TReturnedData> ToServiceProcessingResult(ProcessingError processingError)
        {
            var processingResult = new ServiceProcessingResult<TReturnedData>
            {
                IsSuccessful = IsSuccessful,
                Data = Data
            };
            if (!IsSuccessful)
            {
                processingResult.Error = Error.CanBeFixedByUser ? Error : processingError;
            }

            return processingResult;
        }
    }

    public class PagedDataAccessResult<TReturnedData> : DataAccessResult<TReturnedData>
    {
        public bool HasNext { get; set; }

        public bool HasPrevious { get; set; }

        public int Count { get; set; }

        public int CurrentPage { get; set; }
    }
}
