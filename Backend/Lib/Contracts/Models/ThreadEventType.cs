using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Contracts.Models
{
    // This model represents : “What kind of change happened”
    //Used by: ThreadStateManager
    public enum ThreadEventType
    {
        // Request lifecycle
        RequestStarted,
        RequestCompleted,
        RequestSwitched,

        // Method lifecycle
        MethodEntered,
        MethodExited,
        MethodException,

        // Thread state
        ThreadBecameBusy,
        ThreadBecameIdle
    }
}
