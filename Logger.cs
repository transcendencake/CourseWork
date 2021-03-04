using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseWork
{
    public static class Logger
    {
        public static ILogger DebugLogger;
        
        static Logger()
        {
            var loggerFactory = LoggerFactory.Create(loggingBuilder => { loggingBuilder.AddDebug(); });
            DebugLogger = loggerFactory.CreateLogger<Startup>();
        }
    }
}
