#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Volume.Mpr
{
	public interface IVolumeReference : IDisposable
	{
		Volume Volume { get; }

		/// <summary>
		/// Clones an existing <see cref="IVolumeReference"/>, creating a new transient reference.
		/// </summary>
		IVolumeReference Clone();
	}

	partial class Volume
	{
		private class VolumeReference : IVolumeReference
		{
			private Volume _volume;

			public VolumeReference(Volume volume)
			{
				_volume = volume;
				_volume.OnReferenceCreated();
			}

			public Volume Volume
			{
				get { return _volume; }
			}

			public IVolumeReference Clone()
			{
				return _volume.CreateTransientReference();
			}

			public void Dispose()
			{
				if (_volume != null)
				{
					_volume.OnReferenceDisposed();
					_volume = null;
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
					throw new ObjectDisposedException("The underlying sop data source has already been disposed.");

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
		/// Creates a new 'transient reference' to this <see cref="Volume"/>.
		/// </summary>
		/// <remarks>See <see cref="IVolumeReference"/> for a detailed explanation of 'transient references'.</remarks>
		public IVolumeReference CreateTransientReference()
		{
			return new VolumeReference(this);
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