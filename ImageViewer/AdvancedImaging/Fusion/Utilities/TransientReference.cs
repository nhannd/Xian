#region License

// Copyright (c) 2010, ClearCanvas Inc.
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

namespace ClearCanvas.ImageViewer.AdvancedImaging.Fusion.Utilities
{
	public interface ITransientReference<T> : IDisposable where T : class, IDisposable
	{
		T Object { get; }

		/// <summary>
		/// Clones an existing <see cref="ITransientReference{T}"/>, creating a new transient reference.
		/// </summary>
		ITransientReference<T> Clone();
	}

	public class TransientWrapper<T> : IDisposable where T : class, IDisposable
	{
		private class TransientReference : ITransientReference<T>
		{
			private TransientWrapper<T> _transientWrapper;

			public TransientReference(TransientWrapper<T> transientWrapper)
			{
				_transientWrapper = transientWrapper;
				_transientWrapper.OnReferenceCreated();
			}

			public T Object
			{
				get { return _transientWrapper._object; }
			}

			public ITransientReference<T> Clone()
			{
				return _transientWrapper.CreateTransientReference();
			}

			public void Dispose()
			{
				if (_transientWrapper != null)
				{
					_transientWrapper.OnReferenceDisposed();
					_transientWrapper = null;
				}
			}
		}

		private readonly object _syncLock = new object();
		private int _transientReferenceCount = 0;
		private bool _selfDisposed = false;
		private T _object;

		public TransientWrapper(T @object)
		{
			_object = @object;
		}

		~TransientWrapper()
		{
			this.Dispose(false);
		}

		public T Object
		{
			get { return _object; }
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_object != null)
				{
					_object.Dispose();
					_object = null;
				}
			}
		}

		private void OnReferenceDisposed()
		{
			lock (_syncLock)
			{
				if (_transientReferenceCount > 0)
					--_transientReferenceCount;

				if (_transientReferenceCount == 0 && _selfDisposed)
					DisposeInternal();
			}
		}

		private void OnReferenceCreated()
		{
			lock (_syncLock)
			{
				if (_transientReferenceCount == 0 && _selfDisposed)
					throw new ObjectDisposedException(string.Empty);

				++_transientReferenceCount;
			}
		}

		private void DisposeInternal()
		{
			try
			{
				this.Dispose(true);
				GC.SuppressFinalize(this);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Warn, e);
			}
		}

		/// <summary>
		/// Creates a new 'transient reference' to this <see cref="TransientWrapper{T}"/>.
		/// </summary>
		/// <remarks>See <see cref="ITransientReference{T}"/> for a detailed explanation of 'transient references'.</remarks>
		public ITransientReference<T> CreateTransientReference()
		{
			return new TransientReference(this);
		}

		/// <summary>
		/// Implementation of the <see cref="System.IDisposable"/> pattern.
		/// </summary>
		public void Dispose()
		{
			lock (_syncLock)
			{
				_selfDisposed = true;

				//Only dispose for real when self has been disposed and all the transient references have been disposed.
				if (_transientReferenceCount == 0)
					DisposeInternal();
			}
		}
	}
}