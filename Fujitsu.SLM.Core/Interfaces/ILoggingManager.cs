using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fujitsu.SLM.Enumerations;

namespace Fujitsu.SLM.Core.Interfaces
{
    public interface ILoggingManager
    {
        void Write(LoggingEventSource loggingEventSource,
            LoggingEventType loggingEventType,
            string message);
    }
}
