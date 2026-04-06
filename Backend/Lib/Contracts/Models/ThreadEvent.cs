using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Contracts.Models
{
    //This model represents : “What we SEND to frontend via SignalR”
    public class ThreadEvent
    {
        public int ThreadId { get; set; }

        public string RequestId { get; set; }

        public ThreadEventType EventType { get; set; } 

        public ExecutionInfo? Execution { get; set; }

        public ThreadStateInfo State { get; set; }

        public DateTime Timestamp { get; set; }

    }
}
