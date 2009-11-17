using System;
using System.Threading;

namespace ClearCanvas.ImageViewer.Common
{
	public enum RegenerationCost
	{
		Low = 0,
		Medium = 1,
		High = 2
	}

	public class LargeObjectContainerData : ILargeObjectContainer
	{
		private readonly Guid _identifier;
		private int _lockCount;
		private volatile int _largeObjectCount;
		private long _totalBytesHeld;
		private volatile RegenerationCost _regenerationCost;
		private DateTime _lastAccessTime;
		private uint _lastAccessTimeAccuracyMilliseconds = 500;
		private int _lastAccessUpdateTickCount;

		public LargeObjectContainerData(Guid identifier)
		{
			_identifier = identifier;
		}

		#region ILargeObjectContainer Members

		public Guid Identifier
		{
			get { return _identifier; }
		}

		public int LargeObjectCount
		{
			get { return _largeObjectCount; }
			set { _largeObjectCount = value; }
		}

		public long BytesHeldCount
		{
			get { return _totalBytesHeld; }
			set { _totalBytesHeld = value; }
		}

		public uint LastAccessTimeAccuracyMilliseconds
		{
			get { return _lastAccessTimeAccuracyMilliseconds; }
			set { _lastAccessTimeAccuracyMilliseconds = value; }
		}

		public DateTime LastAccessTime
		{
			get { return _lastAccessTime; }
		}

		public RegenerationCost RegenerationCost
		{
			get { return _regenerationCost; }
			set { _regenerationCost = value; }
		}

		public bool IsLocked
		{
			get { return Thread.VolatileRead(ref _lockCount) > 0; }
		}

		public void UpdateLastAccessTime()
		{
			//DateTime.Now is extremely expensive if called in a tight loop, so we minimize the potential impact
			//of this problem occurring by only updating the last access time every second or so.
			if (Environment.TickCount - _lastAccessUpdateTickCount < _lastAccessTimeAccuracyMilliseconds)
				return;

			_lastAccessUpdateTickCount = Environment.TickCount;
			_lastAccessTime = DateTime.Now;
		}

		public void Lock()
		{
			Interlocked.Increment(ref _lockCount);
		}

		public void Unlock()
		{
			Interlocked.Decrement(ref _lockCount);
		}

		public void Unload()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion
	}

	public interface ILargeObjectContainer
	{
		Guid Identifier { get; }

		int LargeObjectCount { get; }
		long BytesHeldCount { get; }

		DateTime LastAccessTime { get; }
		RegenerationCost RegenerationCost { get; }
		bool IsLocked { get; }

		void Lock();
		void Unlock();

		void Unload();
	}
}
