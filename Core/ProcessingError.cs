using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class ProcessingError
    {
        public string UserMessage { get; set; }
        public string UserHelp { get; set; }
        public bool IsFatal { get; set; }
        public bool CanBeFixedByUser { get; set; }

        public ProcessingError(string userMessage, string userHelp, bool isFatal, bool canBeFixedByUser = false)
        {
            UserMessage = userMessage;
            UserHelp = userHelp;
            IsFatal = isFatal;
            CanBeFixedByUser = canBeFixedByUser;
        }
    }
}
