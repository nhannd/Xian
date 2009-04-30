#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

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