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
		private bool _selfDisposed = false;

		private void OnReferenceDisposed()
		{
			lock(_syncLock)
			{
				if (_transientReferenceCount > 0)
					--_transientReferenceCount;

				if (_transientReferenceCount == 0 && _selfDisposed)
					DisposeInternal();
			}
		}
		
		private void OnReferenceCreated()
		{
			lock(_syncLock)
			{
				if (_transientReferenceCount == 0 && _selfDisposed)
					throw new ObjectDisposedException("The underlying sop data source has already been disposed.");

				++_transientReferenceCount;
			}
		}

		private void DisposeInternal()
		{
			try
			{
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
			lock(_syncLock)
			{
				_selfDisposed = true;

				//Only dispose for real when self has been disposed and all the transient references have been disposed.
				if (_transientReferenceCount == 0)
					DisposeInternal();
			}
		}

		#endregion
	}
}