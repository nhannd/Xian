using System.Collections.Generic;
using System.Threading;

namespace ClearCanvas.Ris.Shreds
{
    /// <summary>
    /// Abstract base implementation of <see cref="IProcessor"/>.
    /// </summary>
    /// <remarks>
    /// Would be implementors of <see cref="IProcessor"/> should inherit this class instead to be
    /// shielded from potential changes to the <see cref="IProcessor"/> interface.
    /// </remarks>
	public abstract class ProcessorBase : IProcessor
	{
		private volatile bool _stopRequested;

        protected ProcessorBase()
        {
        }

        #region IProcessor Members

        void IProcessor.Run()
        {
            RunCore();
        }

        void IProcessor.RequestStop()
        {
            _stopRequested = true;
        }

        #endregion

        /// <summary>
        /// Implements the main logic of the processor.
        /// </summary>
        /// <remarks>
        /// Implementation is expected to run indefinitely but must poll the
        /// <see cref="StopRequested"/> property and exit in a timely manner when true.
        /// </remarks>
        protected abstract void RunCore();

        /// <summary>
        /// Gets a value indicating whether this processor has been requested to terminate.
        /// </summary>
        protected bool StopRequested
        {
            get { return _stopRequested; }
        }
	}
}
