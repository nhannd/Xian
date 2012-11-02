#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.Common;
using System.Threading;
using System.Diagnostics;
using DataCache;

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
        private volatile int _frameDataCreateCount;

		/// <summary>
		/// Constructs a new <see cref="StandardSopDataSource"/>.
		/// </summary>
		protected StandardSopDataSource() : base() {}

		/// <summary>
		/// Gets a value indicating whether or not the SOP instance is an image.
		/// </summary>
		public override bool IsImage
		{
			get { return _frameData != null || Sop.IsImageSop(SopClass.GetSopClass(this.SopClassUid)); }
		}

	    /// <summary>
	    /// Called by the base class to create a new <see cref="StandardSopFrameData"/> containing the data for a particular frame in the SOP instance.
	    /// </summary>
	    /// <param name="frameInfo"> </param>
	    /// <returns>A new <see cref="StandardSopFrameData"/> containing the data for a particular frame in the SOP instance.</returns>
	    protected abstract StandardSopFrameData CreateFrameData(FrameInfo frameInfo);

	    /// <summary>
	    /// Gets the data for a particular frame in the SOP instance.
	    /// </summary>
	    /// <param name="frameInfo"> </param>
	    /// <param name="frameNumber">The 1-based number of the frame for which the data is to be retrieved.</param>
	    /// <returns>An <see cref="ISopFrameData"/> containing frame-specific data.</returns>
	    protected override ISopFrameData GetFrameData(FrameInfo frameInfo)
		{
			            if (_frameData == null || _frameDataCreateCount < _frameData.Length)
            {
                lock (SyncLock)
                {
                    if (_frameData == null)
                    {
                        _frameData = new ISopFrameData[NumberOfFrames];
                    }
                    if (_frameData[frameInfo.FrameNumber - 1] == null)
                    {
                        _frameData[frameInfo.FrameNumber - 1] = CreateFrameData(frameInfo);
                        _frameDataCreateCount++;
                    }
                }
            }
                        return _frameData[frameInfo.FrameNumber - 1];
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
					foreach (var frame in frameData)
					{
					    if (frame != null)
                            frame.Dispose();
					}
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

			private readonly Dictionary<int, byte[]> _overlayData = new Dictionary<int, byte[]>(16);
		    protected readonly FrameInfo FrameInfo;

			protected readonly LargeObjectContainerData _largeObjectContainerData = new LargeObjectContainerData(Guid.NewGuid());

			/// <summary>
			/// Constructs a new <see cref="StandardSopFrameData"/>
			/// </summary>
			/// <param name="frameNumber">The 1-based number of this frame.</param>
			/// <param name="parent">The parent <see cref="ISopDataSource"/> that this frame belongs to.</param>
			/// <exception cref="ArgumentNullException">Thrown if <paramref name="parent"/> is null.</exception>
			/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="frameNumber"/> is zero or negative.</exception>
			protected StandardSopFrameData(FrameInfo frameInfo, StandardSopDataSource parent)
				: this(frameInfo, parent, RegenerationCost.Medium)
			{
			}

		    /// <summary>
		    /// Constructs a new <see cref="StandardSopFrameData"/>.
		    /// </summary>
		    /// <param name="frameInfo"> </param>
		    /// <param name="parent">The parent <see cref="ISopDataSource"/> that this frame belongs to.</param>
		    /// <param name="regenerationCost">The approximate cost to regenerate the pixel and/or overlay data.</param>
		    /// <exception cref="ArgumentNullException">Thrown if <paramref name="parent"/> is null.</exception>
		    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="frameNumber"/> is zero or negative.</exception>
		    protected StandardSopFrameData(FrameInfo frameInfo, StandardSopDataSource parent, RegenerationCost regenerationCost) 
				: base(frameInfo.FrameNumber, parent)
			{
				_largeObjectContainerData.RegenerationCost = regenerationCost;
		        FrameInfo = frameInfo;
			}

			/// <summary>
			/// Gets the parent <see cref="StandardSopDataSource"/> to which this frame belongs.
			/// </summary>
			public new StandardSopDataSource Parent
			{
				get { return (StandardSopDataSource) base.Parent; }
			}

			/// <summary>
			/// Gets or sets the approximate cost to regenerate the pixel and/or overlay data.
			/// </summary>
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
				_largeObjectContainerData.UpdateLastAccessTime();

			    var cached = Frame.Cache.Get(FrameInfo.CacheId);
                if (cached != null)
			      return cached.Data;

                byte[] pixelData = CreateNormalizedPixelData();

                Frame.Cache.Put(FrameInfo.CacheId, new ByteBufferCacheItem()
                {
                    Size = pixelData.Length,
                    Data = pixelData,
                    IsCompressed = false
                });
			    return pixelData;
			}

			protected void UpdateLargeObjectInfo()
			{
				_largeObjectContainerData.LargeObjectCount += _overlayData.Count;
				foreach (KeyValuePair<int, byte[]> pair in _overlayData)
				{
					if (pair.Value != null)
						_largeObjectContainerData.BytesHeldCount += pair.Value.Length;
				}
			}

			/// <summary>
			/// Called by <see cref="GetNormalizedPixelData"/> to create a new byte buffer
			/// containing normalized pixel data for this frame (8 or 16-bit grayscale, or 32-bit ARGB).
			/// </summary>
			/// <remarks>
			/// See <see cref="GetNormalizedPixelData"/> for details on the expected format of the byte buffer.
			/// </remarks>
			/// <returns>A new byte buffer containing the normalized pixel data.</returns>
			protected abstract byte[] CreateNormalizedPixelData();

			/// <summary>
			/// Gets the normalized overlay data buffer for a particular overlay group (8-bit grayscale).
			/// </summary>
			/// <remarks>
			/// <para>
			/// <i>Normalized</i> overlay data means that the 1-bit overlay pixel data is extracted and
			/// unpacked as necessary to form an 8-bit-per-pixel buffer with values of either 0 or 255.
			/// </para>
			/// <para>
			/// Ensuring that the overlay data always meets the above criteria allows clients to easily
			/// consume overlay data without having to worry about the storage of overlay data, whether
			/// embedded in unused bits of the pixel data or in a separate packed bits buffer.
			/// </para>
			/// <para>
			/// Overlay data is reloaded when this method is called after a call to <see cref="ISopFrameData.Unload"/>.
			/// The pixel data will also be reloaded if this method is called before
			/// <see cref="ISopFrameData.GetNormalizedPixelData"/> and there are overlays stored in unused bits of the
			/// pixel data.
			/// </para>
			/// </remarks>
			/// <param name="overlayNumber">The 1-based overlay plane number.</param>
			/// <returns>A byte buffer containing the normalized overlay pixel data.</returns>
			/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="overlayNumber"/> is not a positive non-zero number.</exception>
			public override byte[] GetNormalizedOverlayData(int overlayNumber)
			{
				_largeObjectContainerData.UpdateLastAccessTime();

				if (overlayNumber < 1)
					throw new ArgumentOutOfRangeException("overlayNumber", overlayNumber, "Overlay number must be a positive, non-zero number.");

				lock (SyncLock)
				{
					byte[] data;
					if (!_overlayData.TryGetValue(overlayNumber, out data) || data == null)
					{
						_overlayData[overlayNumber] = data = CreateNormalizedOverlayData(overlayNumber);
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
			/// overlay pixel data for a particular overlay plane.
			/// </summary>
			/// <remarks>
			/// See <see cref="GetNormalizedOverlayData"/> for details on the expected format of the byte buffer.
			/// </remarks>
			/// <param name="overlayNumber">The 1-based overlay plane number.</param>
			/// <returns>A new byte buffer containing the normalized overlay pixel data.</returns>
			protected abstract byte[] CreateNormalizedOverlayData(int overlayNumber);

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
					_largeObjectContainerData.UpdateLastAccessTime();

					ReportLargeObjectsUnloaded();
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

                Frame.Cache.ClearFromMemory(FrameInfo.CacheId);
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
                    Unload();
					lock (this.SyncLock)
					{
						ReportLargeObjectsUnloaded();
						_overlayData.Clear();

						MemoryManager.Remove(this);
					}
				}
			}

			private void ReportLargeObjectsUnloaded()
			{
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
