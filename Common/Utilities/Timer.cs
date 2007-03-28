using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ClearCanvas.Common.Utilities
{
	public delegate void TimerDelegate();

	public class Timer : IDisposable
	{
		private System.Threading.Timer _internalTimer;
		private object _syncLock = new object();
		private InterthreadMarshaler _marshaler;

		public Timer(TimerDelegate expiredDelegate, int dueTime, int period)
		{
			TimerCallback internalTimerCallback = 
				delegate(object stateObject)
				{
					InvokeDelegate marshalDelegate =
						delegate()
						{
							expiredDelegate();
						};

					lock (_syncLock)
					{
						if (_marshaler != null)
							_marshaler.QueueInvoke(marshalDelegate);
					}
				};

			_marshaler = new InterthreadMarshaler();
			_internalTimer = new System.Threading.Timer(internalTimerCallback, null, dueTime, period);
		}

		protected Timer()
		{
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_internalTimer != null)
				{
					_internalTimer.Dispose();
					_internalTimer = null;
					
					lock (_syncLock)
					{
						_marshaler.Dispose();
						_marshaler = null;
					}
				}
			}
		}

		#region IDisposable Members

		public void Dispose()
		{
			try
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
			catch (Exception e)
			{
				Platform.Log(e);
			}
		}

		#endregion
	}
}
