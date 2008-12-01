using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Ris.Shreds
{
    /// <summary>
    /// Defines an interface to a processor that is executed by a shred.
    /// </summary>
    public interface IProcessor
    {
        /// <summary>
        /// Runs the processor.  This method is expected to block indefinitely until
        /// the <see cref="RequestStop"/> method is called.
        /// </summary>
        void Run();

        /// <summary>
        /// Requests the task to exit gracefully.  This method will be called
        /// on a thread other than the thread on which the task is executing.
        /// </summary>
        /// <remarks>
        /// The <see cref="Run"/> method must be implemented in such a way as to heed
        /// a request to stop within a timely manner.
        /// </remarks>
        void RequestStop();
    }
}
