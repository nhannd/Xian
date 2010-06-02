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

namespace ClearCanvas.ImageViewer.AdvancedImaging.Fusion
{
	public interface IFusionOverlaySliceReference : IDisposable
	{
		FusionOverlaySlice FusionOverlaySlice { get; }

		/// <summary>
		/// Clones an existing <see cref="IFusionOverlaySliceReference"/>, creating a new transient reference.
		/// </summary>
		IFusionOverlaySliceReference Clone();
	}

	partial class FusionOverlaySlice
	{
		private class FusionOverlaySliceReference : IFusionOverlaySliceReference
		{
			private FusionOverlaySlice _slice;

			public FusionOverlaySliceReference(FusionOverlaySlice data)
			{
				_slice = data;
				_slice.OnReferenceCreated();
			}

			public FusionOverlaySlice FusionOverlaySlice
			{
				get { return _slice; }
			}

			public IFusionOverlaySliceReference Clone()
			{
				return _slice.CreateTransientReference();
			}

			public void Dispose()
			{
				if (_slice != null)
				{
					_slice.OnReferenceDisposed();
					_slice = null;
				}
			}
		}

		private readonly object _syncLock = new object();
		private int _transientReferenceCount = 0;
		private bool _selfDisposed = false;

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
					throw new ObjectDisposedException("");

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
		/// Creates a new 'transient reference' to this <see cref="FusionOverlaySlice"/>.
		/// </summary>
		/// <remarks>See <see cref="IFusionOverlaySliceReference"/> for a detailed explanation of 'transient references'.</remarks>
		public IFusionOverlaySliceReference CreateTransientReference()
		{
			return new FusionOverlaySliceReference(this);
		}

		/// <summary>
		/// Implementation of the <see cref="IDisposable"/> pattern.
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