#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.AdvancedImaging.Fusion
{
	public interface IFusionOverlayFrameDataReference : IDisposable
	{
		FusionOverlayFrameData FusionOverlayFrameData { get; }

		/// <summary>
		/// Clones an existing <see cref="IFusionOverlayFrameDataReference"/>, creating a new transient reference.
		/// </summary>
		IFusionOverlayFrameDataReference Clone();
	}

	//TODO (CR Sept 2010): These references don't need to be "transient".  Transient references
	//should only be for objects that must continue to "live" after they've been disposed because they can't
	//be hidden behind some interface via a factory, for example.
	partial class FusionOverlayFrameData
	{
		private class FusionOverlayFrameDataReference : IFusionOverlayFrameDataReference
		{
			private FusionOverlayFrameData _frameData;

			public FusionOverlayFrameDataReference(FusionOverlayFrameData data)
			{
				_frameData = data;
				_frameData.OnReferenceCreated();
			}

			public FusionOverlayFrameData FusionOverlayFrameData
			{
				get { return _frameData; }
			}

			public IFusionOverlayFrameDataReference Clone()
			{
				return _frameData.CreateTransientReference();
			}

			public void Dispose()
			{
				if (_frameData != null)
				{
					_frameData.OnReferenceDisposed();
					_frameData = null;
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
		/// Creates a new 'transient reference' to this <see cref="FusionOverlayFrameData"/>.
		/// </summary>
		/// <remarks>See <see cref="IFusionOverlayFrameDataReference"/> for a detailed explanation of 'transient references'.</remarks>
		public IFusionOverlayFrameDataReference CreateTransientReference()
		{
			return new FusionOverlayFrameDataReference(this);
		}

		/// <summary>
		/// Implementation of the <see cref="IDisposable"/> pattern.
		/// </summary>
		public void Dispose()
		{
			lock (_syncLock)
			{
				//TODO (CR Sept 2010): seems there's no owner of "this", so _selfDisposed will never be set to true, and this
				//object will never be disposed.
				_selfDisposed = true;

				//Only dispose for real when self has been disposed and all the transient references have been disposed.
				if (_transientReferenceCount == 0)
					DisposeInternal();
			}
		}
	}
}