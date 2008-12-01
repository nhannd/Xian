using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Ris.Shreds
{
    /// <summary>
    /// Defines an interface to a processor that is executed by a shred.
    /// </summary>
    /// <remarks>
    /// The implementation should not make use of threads.  All threading is handled externally.
    /// The <see cref="Run"/> method will be invoked on one thread, and the <see cref="RequestStop"/>
    /// method invoked on another thread.
    /// </remarks>
    public interface IProcessor
    {
        /// <summary>
        /// Runs the processor.
        /// </summary>
        /// <remarks>
        /// This method is expected to block indefinitely until the <see cref="RequestStop"/>
        /// method is called, at which point it should exit in a timely manner.
        /// </remarks>
        void Run();

        /// <summary>
        /// Requests the task to exit gracefully.
        /// </summary>
        /// <remarks>
        /// This method will be called on a thread other than the thread on which the task is executing.
        /// This method should return quickly - it should not block.  A typical implementation simply
        /// sets a flag that causes the <see cref="Run"/> method to terminate.
        /// must be implemented in such a way as to heed
        /// a request to stop within a timely manner.
        /// </remarks>
        void RequestStop();
    }
}
