using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Ris.Shreds
{
    /// <summary>
    /// A specialization of <see cref="ProcessorBase"/> that is designed to process
    /// a queue of items.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    /// <remarks>
    /// This class implements the logic to process a queue of items.  It polls the queue
    /// for a batch of items to process, processes those items, and then polls the queue
    /// again.  If the queue is empty, it sleeps for a preset amount of time.
    /// </remarks>
    public abstract class QueueProcessor<TItem> : ProcessorBase
    {
        private const int SnoozeIntervalInMilliseconds = 100;

        private int _batchSize;
        private TimeSpan _sleepTime;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="batchSize">Max number of items to pull off queue for processing.</param>
        /// <param name="sleepTime"></param>
        protected QueueProcessor(int batchSize, TimeSpan sleepTime)
        {
            _batchSize = batchSize;
            _sleepTime = sleepTime;
        }

        /// <summary>
        /// Gets the next batch of items from the queue.
        /// </summary>
        /// <param name="batchSize"></param>
        /// <returns></returns>
        protected abstract IList<TItem> GetNextBatch(int batchSize);

        /// <summary>
        /// Processes a single item.
        /// </summary>
        /// <param name="item"></param>
        protected abstract void ProcessItem(TItem item);

        #region Helpers

        protected override void RunCore()
        {
            while (!StopRequested)
            {
                IList<TItem> items;
                try
                {
                    // try to get a batch of items
                    items  = GetNextBatch(_batchSize);
                }
                catch (Exception e)
                {
                    // this exception may be transient, so we just log it and continue
                    // as if the queue is empty
					ExceptionLogger.Log(this.GetType().FullName + ".GetNextBatch", e);
                    items = new List<TItem>();
                }
                
                // if no items, sleep
                if (items.Count == 0 && !StopRequested)
                {
                    Sleep();
                }
                else
                {
                    // process each item
                    foreach (TItem item in items)
                    {
                        // break if stop requested
                        // (unprocessed items will remain in queue and be picked up next time)
                        if (StopRequested)
                            break;

                        try
                        {
                            // process the item
                            ProcessItem(item);
                        }
                        catch (Exception e)
                        {
                            // nothing we can do about this, just log it and process next item
							ExceptionLogger.Log(this.GetType().FullName + ".ProcessItem", e);
						}
                    }
                }
            }
        }

        private void Sleep()
        {
            // sleep for the total sleep time, unless stop requested
            for (int i = 0; i < _sleepTime.TotalMilliseconds 
                && !StopRequested; i += SnoozeIntervalInMilliseconds)
            {
                Thread.Sleep(SnoozeIntervalInMilliseconds);
            }
        }

        #endregion

    }
}
