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
		where TItem : Entity
    {
        private const int SnoozeIntervalInMilliseconds = 100;

        private readonly int _batchSize;
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
        /// <remarks>
        /// Subclasses can assume that a read-scope has been established.
        /// </remarks>
        /// <param name="batchSize"></param>
        /// <returns></returns>
        protected abstract IList<TItem> GetNextBatch(int batchSize);

		/// <summary>
		/// Called to act on a queue item.
		/// </summary>
		/// <remarks>
		/// This method is intended to take actions based on the item, but must not modify the item.  If
		/// the item cannot be fully acted on, an exception should be thrown.
		/// </remarks>
		/// <param name="item"></param>
		protected abstract void ActOnItem(TItem item);

		/// <summary>
		/// Called when <see cref="ActOnItem"/> succeeds.
		/// </summary>
		/// <remarks>
		/// This method is intended to update the item to indicate that the actions succeeded.
		/// Subclasses can assume that an update context has been established.
		/// </remarks>
		/// <param name="item"></param>
    	protected abstract void OnItemSucceeded(TItem item);

		/// <summary>
		/// Called when <see cref="ActOnItem"/> throws an exception.
		/// </summary>
		/// <remarks>
		/// This method is intended to update the time to indicate that processing failed, with the 
		/// specified error.  Subclasses can assume that an update context has been established.
		/// </remarks>
		/// <param name="item"></param>
		/// <param name="error"></param>
    	protected abstract void OnItemFailed(TItem item, Exception error);

        #region Helpers

        protected override void RunCore()
        {
            while (!StopRequested)
            {
                IList<TItem> items;
                try
                {
                    // try to get a batch of items
                    items  = GetBatch();
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

		private IList<TItem> GetBatch()
		{
			using (PersistenceScope scope = new PersistenceScope(PersistenceContextType.Read))
			{
				IList<TItem> items = GetNextBatch(_batchSize);

				scope.Complete();
				return items;
			}
		}

		private void ProcessItem(TItem item)
		{
			Exception error = null;
			using (PersistenceScope scope = new PersistenceScope(PersistenceContextType.Update))
			{
				IUpdateContext context = (IUpdateContext)PersistenceScope.CurrentContext;
				context.ChangeSetRecorder.OperationName = this.GetType().FullName;

				// need to lock the item in context, to allow loading of extended properties collection by subclass
				context.Lock(item);

				try
				{
					// take action base on item
					ActOnItem(item);

					// ensure that the commit will ultimately succeed
					context.SynchState();

					// success callback
					OnItemSucceeded(item);

					// complete the transaction
					scope.Complete();
				}
				catch (Exception e)
				{
					// one of the actions failed
					ExceptionLogger.Log(this.GetType().FullName + ".ProcessItem", e);
					error = e;
				}
			}

			if (error != null)
			{
				// use a new scope to mark the item as failed
				using (PersistenceScope scope = new PersistenceScope(PersistenceContextType.Update, PersistenceScopeOption.RequiresNew))
				{
					IUpdateContext failContext = (IUpdateContext)PersistenceScope.CurrentContext;
					failContext.ChangeSetRecorder.OperationName = this.GetType().FullName;

					// lock item into this context
					failContext.Lock(item);

					// failure callback
					OnItemFailed(item, error);

					// complete the transaction
					scope.Complete();
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
