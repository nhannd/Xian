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
using System.Collections.Generic;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.Common;
using System.Threading;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// Base implementation of a <see cref="SopDataSource"/> with built-in resource management.
	/// </summary>
	public abstract class StandardSopDataSource : SopDataSource
	{
		// I hate doing this, but it's horribly inefficient for all subclasses to do their own locking.

		/// <summary>
		/// Gets a lock object suitable for synchronizing access to the data source.
		/// </summary>
		protected readonly object SyncLock = new object();

		private volatile ISopFrameData[] _frameData;

		/// <summary>
		/// Constructs a new <see cref="StandardSopDataSource"/>.
		/// </summary>
		protected StandardSopDataSource() : base() {}

		/// <summary>
		/// Gets a value indicating whether or not the SOP instance is an image.
		/// </summary>
		public override bool IsImage
		{
			get { return Sop.IsImageSop(SopClass.GetSopClass(this.SopClassUid)); }
		}

		/// <summary>
		/// Called by the base class to create a new <see cref="StandardSopFrameData"/> containing the data for a particular frame in the SOP instance.
		/// </summary>
		/// <param name="frameNumber">The 1-based number of the frame for which the data is to be retrieved.</param>
		/// <returns>A new <see cref="StandardSopFrameData"/> containing the data for a particular frame in the SOP instance.</returns>
		protected abstract StandardSopFrameData CreateFrameData(int frameNumber);

		/// <summary>
		/// Gets the data for a particular frame in the SOP instance.
		/// </summary>
		/// <param name="frameNumber">The 1-based number of the frame for which the data is to be retrieved.</param>
		/// <returns>An <see cref="ISopFrameData"/> containing frame-specific data.</returns>
		protected override ISopFrameData GetFrameData(int frameNumber)
		{
			if(_frameData == null)
			{
				lock(this.SyncLock)
				{
					if(_frameData == null)
					{
						_frameData = new ISopFrameData[this.NumberOfFrames];
						for (int n = 0; n < _frameData.Length; n++)
							_frameData[n] = this.CreateFrameData(n + 1);
					}
				}
			}

			return _frameData[frameNumber - 1];
		}

		/// <summary>
		/// Implementation of the <see cref="IDisposable"/> pattern.
		/// </summary>
		/// <param name="disposing">A value indicating whether or not the object is being disposed.</param>
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);

			if (disposing)
			{
				ISopFrameData[] frameData;
				lock(SyncLock)
				{
					frameData = _frameData;
					_frameData = null;
				}

				if (frameData != null)
				{
					foreach (ISopFrameData frame in frameData)
						frame.Dispose();
				}
			}
		}

		#region StandardSopFrameData class

		/// <summary>
		/// Base implementation of a <see cref="SopFrameData"/> with built-in resource mamangement.
		/// </summary>
		protected abstract class StandardSopFrameData : SopFrameData, ILargeObjectContainer
		{
			/// <summary>
			/// Gets a lock object suitable for synchronizing access to the frame data.
			/// </summary>
			protected readonly object SyncLock = new object();

			private readonly Dictionary<int, byte[]> _overlayData = new Dictionary<int, byte[]>();
			private volatile byte[] _pixelData = null;

			private readonly LargeObjectContainerData _largeObjectContainerData = new LargeObjectContainerData(Guid.NewGuid());

			/// <summary>
			/// Constructs a new <see cref="StandardSopFrameData"/>
			/// </summary>
			/// <param name="frameNumber">The 1-based number of this frame.</param>
			/// <param name="parent">The parent <see cref="ISopDataSource"/> that this frame belongs to.</param>
			/// <exception cref="ArgumentNullException">Thrown if <paramref name="parent"/> is null.</exception>
			/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="frameNumber"/> is zero or negative.</exception>
			protected StandardSopFrameData(int frameNumber, StandardSopDataSource parent)
				: this(frameNumber, parent, RegenerationCost.Low)
			{
			}

			protected StandardSopFrameData(int frameNumber, StandardSopDataSource parent, RegenerationCost regenerationCost) 
				: base(frameNumber, parent)
			{
				_largeObjectContainerData.RegenerationCost = regenerationCost;
			}

			/// <summary>
			/// Gets the parent <see cref="StandardSopDataSource"/> to which this frame belongs.
			/// </summary>
			public new StandardSopDataSource Parent
			{
				get { return (StandardSopDataSource) base.Parent; }
			}

			protected RegenerationCost RegenerationCost
			{
				get { return _largeObjectContainerData.RegenerationCost; }
				set { _largeObjectContainerData.RegenerationCost = value; }
			}

			/// <summary>
			/// Gets pixel data in normalized form (8 or 16-bit grayscale, or ARGB).
			/// </summary>
			/// <returns></returns>
			/// <remarks>
			/// <i>Normalized</i> pixel data means that:
			/// <list type="Bullet">
			/// <item>
			/// <description>Grayscale pixel data has embedded overlays removed and each pixel value
			/// is padded so that it can be cast directly to the appropriate type (e.g. byte, sbyte, ushort, short).</description>
			/// </item>
			/// <item>
			/// <description>Colour pixel data is always converted into ARGB format.</description>
			/// </item>
			/// <item>
			/// <description>Pixel data is always uncompressed.</description>
			/// </item>
			/// </list>
			/// <para>
			/// Ensuring that the pixel data always meets the above criteria
			/// allows clients to easily consume pixel data without having
			/// to worry about the the multitude of DICOM photometric interpretations
			/// and transfer syntaxes.
			/// </para>
			/// <para>
			/// Pixel data is reloaded when this method is called after a 
			/// call to <see cref="ISopFrameData.Unload"/>.
			/// </para>
			/// </remarks>		
			public override byte[] GetNormalizedPixelData()
			{
				//TODO (cr Oct 2009): do we need to use Platform.Time?
				_largeObjectContainerData.LastAccessTime = DateTime.Now;

				byte[] pixelData = _pixelData;
				if (pixelData == null)
				{
					lock (this.SyncLock)
					{
						pixelData = _pixelData;
						if (pixelData == null)
						{
							pixelData = _pixelData = CreateNormalizedPixelData();
							if (pixelData != null)
							{
								Platform.Log(LogLevel.Debug, "Created pixel data of length {0}", pixelData.Length);
								UpdateLargeObjectInfo();
								MemoryManager.Add(this);
								Diagnostics.OnLargeObjectAllocated(pixelData.Length);
							}
						}
					}
				}

				return pixelData;
			}

			private void UpdateLargeObjectInfo()
			{
				if (_pixelData == null)
				{
					_largeObjectContainerData.LargeObjectCount = 0;
					_largeObjectContainerData.BytesHeldCount = 0;
				}
				else
				{
					_largeObjectContainerData.LargeObjectCount = 1;
					_largeObjectContainerData.BytesHeldCount = _pixelData.Length;
				}

				_largeObjectContainerData.LargeObjectCount += _overlayData.Count;
				foreach (KeyValuePair<int, byte[]> pair in _overlayData)
				{
					if (pair.Value != null)
						_largeObjectContainerData.BytesHeldCount += (long)pair.Value.Length;
				}
			}

			/// <summary>
			/// Called by <see cref="GetNormalizedPixelData"/> to create a new byte buffer
			/// containing normalized pixel data for this frame (8 or 16-bit grayscale, or 32-bit ARGB).
			/// </summary>
			/// <returns>A new byte buffer containing the normalized pixel data.</returns>
			protected abstract byte[] CreateNormalizedPixelData();

			/// <summary>
			/// Gets the normalized overlay pixel data buffer for a particular overlay frame
			/// that is applicable to this image frame (8 or 16-bit grayscale, or 32-bit ARGB).
			/// </summary>
			/// <param name="overlayGroupNumber">The group number of the overlay plane (1-16).</param>
			/// <param name="overlayFrameNumber">The 1-based frame number of the overlay frame to be retrieved.</param>
			/// <returns>A byte buffer containing the normalized overlay pixel data.</returns>
			public override byte[] GetNormalizedOverlayData(int overlayGroupNumber, int overlayFrameNumber)
			{
				_largeObjectContainerData.LastAccessTime = DateTime.Now;

				if(overlayGroupNumber < 1)
					throw new ArgumentOutOfRangeException("overlayGroupNumber", overlayGroupNumber, "Must be a positive, non-zero number.");
				if (overlayFrameNumber < 1)
					throw new ArgumentOutOfRangeException("overlayFrameNumber", overlayFrameNumber, "Must be a positive, non-zero number.");

				int key = ((overlayFrameNumber - 1) << 8) | ((overlayGroupNumber - 1) & 0x000000ff);

				lock (this.SyncLock)
				{
					byte[] data;
					if (!_overlayData.TryGetValue(key, out data) || data == null)
					{
						_overlayData[key] = data = CreateNormalizedOverlayData(overlayGroupNumber, overlayFrameNumber);
						if (data != null)
						{
							UpdateLargeObjectInfo();
							MemoryManager.Add(this);
							Diagnostics.OnLargeObjectAllocated(data.Length);
						}
					}

					return data;
				}
			}

			/// <summary>
			/// Called by <see cref="GetNormalizedOverlayData"/> to create a new byte buffer containing normalized 
			/// overlay pixel data for a particular overlay frame that is applicable to this image frame.
			/// </summary>
			/// <param name="overlayGroupNumber">The group number of the overlay plane (1-16).</param>
			/// <param name="overlayFrameNumber">The 1-based frame number of the overlay frame to be retrieved.</param>
			/// <returns>A new byte buffer containing the normalized overlay pixel data.</returns>
			protected abstract byte[] CreateNormalizedOverlayData(int overlayGroupNumber, int overlayFrameNumber);

			/// <summary>
			/// Unloads any cached byte buffers owned by this <see cref="ISopFrameData"/>.
			/// </summary>
			/// <remarks>
			/// It is sometimes necessary to manage the memory used by unloading the pixel data. 
			/// Calling this method will not necessarily result in an immediate decrease in memory
			/// usage, since it merely releases the reference to the pixel data; it is up to the
			/// garbage collector to free the memory.  Calling <see cref="ISopFrameData.GetNormalizedPixelData"/>
			/// will reload the pixel data.
			/// </remarks>
			public override sealed void Unload()
			{
				lock (this.SyncLock)
				{
					_largeObjectContainerData.LastAccessTime = DateTime.Now;

					ReportLargeObjectsUnloaded();

					_pixelData = null;
					_overlayData.Clear();
					this.OnUnloaded();

					UpdateLargeObjectInfo();
					MemoryManager.Remove(this);
				}
			}

			/// <summary>
			/// Called by the base class when the cached byte buffers are being unloaded.
			/// </summary>
			protected virtual void OnUnloaded()
			{
			}

			/// <summary>
			/// Called by the base <see cref="SopFrameData"/> to release any owned resources.
			/// </summary>
			/// <param name="disposing">A value indicating whether or not the object is being disposed.</param>
			protected override void Dispose(bool disposing)
			{
				base.Dispose(disposing);

				if (disposing)
				{
					lock (this.SyncLock)
					{
						ReportLargeObjectsUnloaded();

						_pixelData = null;
						_overlayData.Clear();

						MemoryManager.Remove(this);
					}
				}
			}

			private void ReportLargeObjectsUnloaded()
			{
				if (_pixelData != null)
					Diagnostics.OnLargeObjectReleased(_pixelData.Length);

				foreach (byte[] overlayData in _overlayData.Values)
				{
					if (overlayData != null)
						Diagnostics.OnLargeObjectReleased(overlayData.Length);
				}
			}

			#region ILargeObjectContainer Members

			Guid ILargeObjectContainer.Identifier
			{
				get { return _largeObjectContainerData.Identifier; }
			}

			int ILargeObjectContainer.LargeObjectCount
			{
				get { return _largeObjectContainerData.LargeObjectCount; }
			}

			long ILargeObjectContainer.BytesHeldCount
			{
				get { return _largeObjectContainerData.BytesHeldCount; }
			}

			DateTime ILargeObjectContainer.LastAccessTime
			{
				get { return _largeObjectContainerData.LastAccessTime; }
			}

			RegenerationCost ILargeObjectContainer.RegenerationCost
			{
				get { return _largeObjectContainerData.RegenerationCost; }
			}

			bool ILargeObjectContainer.IsLocked
			{
				get { return _largeObjectContainerData.IsLocked; }
			}

			void ILargeObjectContainer.Lock()
			{
				_largeObjectContainerData.Lock();
			}

			void ILargeObjectContainer.Unlock()
			{
				_largeObjectContainerData.Unlock();
			}

			#endregion
		}

		#endregion
	}
}
