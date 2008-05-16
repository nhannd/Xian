using System;

namespace ClearCanvas.ImageServer.Rules
{
    /// <summary>
    /// The exception that is thrown when the referenced tag used in <see cref="ScheduleServerActionItem"/>
    /// doesn't exist or has no value.
    /// </summary>
    public class ReferencedTagNotFoundException : Exception
    {
        public ReferencedTagNotFoundException(ServerActionContext context, string message)
            : base(message)
        {

        }
    }
}
