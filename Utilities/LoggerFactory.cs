using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;

namespace Utilities
{
    public static class LoggerFactory
    {
        public static ILog GetLogger(Type loggingClassType)
        {
            return LogManager.GetLogger(loggingClassType);
        }

        public static ILog GetLogger(string logName)
        {
            return LogManager.GetLogger(logName);
        }
    }
}
