using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lib.Contracts.Models
{
    // This represents: “Current state of a thread at this moment”
    //Used by:ThreadFlowTracker(to store state) , ThreadStateManager(to compare state)
    public class ThreadSnapshot
    {
        public int ThreadId { get; set; } //Identify thread (UI line)

        public string? RequestId { get; set; } //Track request switching

        public string? ClassName { get; set; } //For visualization

        public string? MethodName { get; set; } //For visualization

        public string? Activity { get; set; } //Controller/Service/etc

        public bool IsBusy { get; set; } //Line state (active/idle)

        public DateTime Timestamp { get; set; } //Ordering + timeline
    }
}
