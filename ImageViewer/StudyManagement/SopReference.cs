using System;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	public interface ISopReference : ISopProvider, IDisposable
	{
		ISopReference Clone();
	}

	public partial class Sop
	{
		private class SopReference : ISopReference
		{
			private Sop _sop;

			public SopReference(Sop sop)
			{
				_sop = sop;
				_sop.OnReferenceCreated();
			}

			#region ISopProvider Members

			public Sop Sop
			{
				get { return _sop; }
			}

			#endregion

			#region ICachedSop Members

			public ISopReference Clone()
			{
				return _sop.CreateTransientReference();
			}

			#endregion

			#region IDisposable Members

			public void Dispose()
			{
				if (_sop != null)
				{
					_sop.OnReferenceDisposed();
					_sop = null;
				}
			}

			#endregion
		}

		#region Sop Stuff for Transient References

		private readonly object _syncLock = new object();
		private int _transientReferenceCount = 0;

		private void OnReferenceDisposed()
		{
			lock(_syncLock)
			{
				if (_transientReferenceCount > 0)
					--_transientReferenceCount;

				if (_transientReferenceCount == 0)
					DisposeInternal();
			}
		}
		
		private void OnReferenceCreated()
		{
			lock(_syncLock)
			{
				if (_transientReferenceCount < 0)
					throw new ObjectDisposedException("The underlying object has already been disposed.");

				++_transientReferenceCount;
			}
		}

		private void DisposeInternal()
		{
			try
			{
				_transientReferenceCount = -1;

				Dispose(true);
				GC.SuppressFinalize(this);
			}
			catch(Exception e)
			{
				Platform.Log(LogLevel.Warn, e);
			}
		}

		public ISopReference CreateTransientReference()
		{
			return new SopReference(this);
		}

		public void Dispose()
		{
			try
			{
				lock(_syncLock)
				{
					//There are transient references out there, so they are now responsible for doing the 'real' disposal.
					if (_transientReferenceCount > 0)
						return;

					DisposeInternal();
				}
			}
			catch(Exception e)
			{
				Platform.Log(LogLevel.Warn, e);
			}
		}

		#endregion
	}
}