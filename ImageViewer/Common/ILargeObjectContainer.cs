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

	internal class LargeObjectContainerData : ILargeObjectContainer
	{
		private readonly string _identifier;
		private int _lockCount;
		private volatile int _largeObjectCount;
		private long _totalBytesHeld;
		private DateTime _lastAccessTime;
		private volatile RegenerationCost _regenerationCost;


		public LargeObjectContainerData(string identifier)
		{
			_identifier = identifier;
		}

		#region ILargeObjectContainer Members

		public string Identifier
		{
			get { return _identifier; }
		}
		public int LargeObjectCount
		{
			get { return _largeObjectCount; }
			set { _largeObjectCount = value; }
		}

		public long TotalBytesHeld
		{
			get { return _totalBytesHeld; }
			set { _totalBytesHeld = value; }
		}

		public DateTime LastAccessTime
		{
			get { return _lastAccessTime; }
			set { _lastAccessTime = value; }

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

	public abstract class LargeObjectContainer : ILargeObjectContainer
	{
		private readonly string _identifier;

		protected LargeObjectContainer(string identifier)
		{
			_identifier = identifier;
		}

		#region ILargeObjectContainer Members

		public string Identifier
		{
			get { return _identifier; }
		}

		public abstract int LargeObjectCount { get;  }

		public abstract long TotalBytesHeld { get;  }

		public abstract DateTime LastAccessTime { get;  }

		public abstract RegenerationCost RegenerationCost { get;  }

		public abstract bool IsLocked { get;  }

		public abstract void Lock();

		public abstract void Unlock();

		public abstract void Unload();

		#endregion
	}

	public interface ILargeObjectContainer
	{
		string Identifier { get; }

		int LargeObjectCount { get; }
		long TotalBytesHeld { get; }

		DateTime LastAccessTime { get; }
		RegenerationCost RegenerationCost { get; }
		bool IsLocked { get; }

		void Lock();
		void Unlock();

		void Unload();
	}
}
