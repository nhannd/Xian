using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Server.ShredHost
{
    /// <summary>
    /// The logic is that a program can be stopped, running, or in transition between stopped and running or vice versa.
    /// During transition, no other state changes are permitted. For example, Thread A attempts to start the ShredHost;
    /// the Start() routine checks to see whether the ShredHost is in a stopped state, because it doesn't make sense to
    /// start a running ShredHost. Once this checks out, the running state is changed to Transition. No methods should be 
    /// allowed to proceed with changing the state while it is in Transition. Once the ShredHost has started up, 
    /// the running state can then be set to Running. If Thread B had tried to stop the ShredHost while it was in Transition
    /// nothing would have happend. IF Thread B tried to stop ShredHost while it is in Running, it will be stopped
    /// successfully.
    /// </summary>
    internal enum RunningState { Stopped, Running, Transition };
}
