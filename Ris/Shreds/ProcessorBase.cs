using System.Collections.Generic;
using System.Threading;

namespace ClearCanvas.Ris.Shreds
{
	public interface IProcessor
	{
		void Initialize(int batchSize, int sleepDurationInSeconds);
		void Start();
		void RequestStop();
	}

	public abstract class ProcessorBase<TItem> : IProcessor
	{
		private volatile bool _shouldStop;
		private int _batchSize;
		private int _sleepDurationInMilliseconds;
		private const int _snoozeIntervalInMilliseconds = 100;
		private Thread _processThread;

		public virtual void Initialize(int batchSize, int sleepDurationInSeconds)
		{
			_batchSize = batchSize;
			_sleepDurationInMilliseconds = sleepDurationInSeconds * 1000;
			_processThread = new Thread(Process);
		}

		public void Start()
		{
			_processThread.Start();
		}

		public void RequestStop()
		{
			_shouldStop = true;
			_processThread.Join();
		}

		public abstract IList<TItem> GetNextBatch(int batchSize);
		public abstract void ProcessItem(TItem item);

		#region Private Helpers

		private void Process()
		{
			while (_shouldStop == false)
			{
				IList<TItem> items = GetNextBatch(_batchSize);

				if (items.Count == 0 && _shouldStop == false)
					Sleep();

				foreach (TItem item in items)
				{
					ProcessItem(item);
					if (_shouldStop)
						break;
				}
			}
		}

		private void Sleep()
		{
			int sleepTimeInMilliseconds = 0;

			while (sleepTimeInMilliseconds < _sleepDurationInMilliseconds)
			{
				Thread.Sleep(_snoozeIntervalInMilliseconds);
				if (_shouldStop)
					break;

				sleepTimeInMilliseconds += _snoozeIntervalInMilliseconds;
			}
		}

		#endregion
	}
}
