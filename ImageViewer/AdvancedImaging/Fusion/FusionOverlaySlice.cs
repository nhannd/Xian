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
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Volume.Mpr;

namespace ClearCanvas.ImageViewer.AdvancedImaging.Fusion
{
	public partial class FusionOverlaySlice : IDisposable, ILargeObjectContainer
	{
		private readonly object _syncPixelDataLock = new object();
		private IFrameReference _baseFrameReference;
		private IFusionOverlayDataReference _overlayDataReference;

		private int _overlayRows, _overlayColumns;
		private int _overlayBitsAllocated, _overlayBitsStored, _overlayHighBit;
		private bool _overlayIsSigned, _overlayIsInverted;
		private double _overlayRescaleSlope, _overlayRescaleIntercept;
		private float _overlayCoregistrationScale, _overlayCoregistrationOffsetX, _overlayCoregistrationOffsetY;
		private byte[] _overlayPixelData;

		public FusionOverlaySlice(Frame baseFrame, FusionOverlayData overlayData)
			: this(baseFrame.CreateTransientReference(), overlayData.CreateTransientReference()) {}

		public FusionOverlaySlice(IFrameReference baseFrame, IFusionOverlayDataReference overlayData)
		{
			_baseFrameReference = baseFrame;
			_overlayDataReference = overlayData;
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_baseFrameReference != null)
				{
					_baseFrameReference.Dispose();
					_baseFrameReference = null;
				}

				if (_overlayDataReference != null)
				{
					_overlayDataReference.Dispose();
					_overlayDataReference = null;
				}
			}
		}

		protected byte[] OverlayPixelData
		{
			get
			{
				// update the last access time
				_largeObjectData.UpdateLastAccessTime();

				// if the data is already available without blocking, return it immediately
				byte[] pixelData = _overlayPixelData;
				if (pixelData != null)
					return pixelData;

				return this.LoadPixelData();
			}
		}

		private byte[] LoadPixelData()
		{
			// wait for synchronized access
			lock (_syncPixelDataLock)
			{
				// if the data is now available, return it immediately
				// (i.e. we were blocked because we were already reading the data)
				if (_overlayPixelData != null)
					return _overlayPixelData;

				// load the pixel data
				_overlayPixelData = this.GenerateOverlaySlice();

				// update our stats
				_largeObjectData.BytesHeldCount = _overlayPixelData.Length;
				_largeObjectData.LargeObjectCount = 1;
				_largeObjectData.UpdateLastAccessTime();

				// regenerating the volume data is easy when the source frames are already in memory!
				_largeObjectData.RegenerationCost = RegenerationCost.Low;

				// register with memory manager
				MemoryManager.Add(this);

				return _overlayPixelData;
			}
		}

		private void UnloadPixelData()
		{
			// wait for synchronized access
			lock (_syncPixelDataLock)
			{
				// dump our data
				_overlayPixelData = null;

				// update our stats
				_largeObjectData.BytesHeldCount = 0;
				_largeObjectData.LargeObjectCount = 0;

				// unregister with memory manager
				MemoryManager.Remove(this);
			}
		}

		private byte[] GenerateOverlaySlice()
		{
			var volume = _overlayDataReference.FusionOverlayData.GetVolume();
			var baseFrame = _baseFrameReference.Frame;

			// compute the bounds of the target base image frame in patient coordinates
			var baseTopLeft = baseFrame.ImagePlaneHelper.ConvertToPatient(new PointF(0, 0));
			var baseTopRight = baseFrame.ImagePlaneHelper.ConvertToPatient(new PointF(baseFrame.Columns, 0));
			var baseBottomLeft = baseFrame.ImagePlaneHelper.ConvertToPatient(new PointF(0, baseFrame.Rows));
			var baseFrameCentre = (baseTopRight + baseBottomLeft)/2;

			// compute the rotated volume slicing basis axes
			var volumeXAxis = (volume.ConvertToVolume(baseTopRight) - volume.ConvertToVolume(baseTopLeft)).Normalize();
			var volumeYAxis = (volume.ConvertToVolume(baseBottomLeft) - volume.ConvertToVolume(baseTopLeft)).Normalize();
			var volumeZAxis = volumeXAxis.Cross(volumeYAxis);

			// the volume slicing transformation matrix is thus just the rotation of the identity basis to the slicing basis
			var volumeSlicerTransform = new Matrix(4, 4);
			volumeSlicerTransform.SetColumn(0, volumeXAxis.X, volumeXAxis.Y, volumeXAxis.Z, 0);
			volumeSlicerTransform.SetColumn(1, volumeYAxis.X, volumeYAxis.Y, volumeYAxis.Z, 0);
			volumeSlicerTransform.SetColumn(2, volumeZAxis.X, volumeZAxis.Y, volumeZAxis.Z, 0);
			volumeSlicerTransform.SetColumn(3, 0, 0, 0, 1);

			var @params = new VolumeSlicerParams(volumeSlicerTransform);
			using (var slice = new VolumeSliceSopDataSource(volume, @params, volume.ConvertToVolume(baseFrameCentre)))
			{
				using (var sliceSop = new ImageSop(slice))
				{
					using (var overlayFrame = sliceSop.Frames[1])
					{
						_overlayRows = overlayFrame.Rows;
						_overlayColumns = overlayFrame.Columns;
						_overlayBitsAllocated = overlayFrame.BitsAllocated;
						_overlayBitsStored = overlayFrame.BitsStored;
						_overlayHighBit = overlayFrame.HighBit;
						_overlayIsSigned = overlayFrame.PixelRepresentation != 0;
						_overlayIsInverted = overlayFrame.PhotometricInterpretation == PhotometricInterpretation.Monochrome1;
						_overlayRescaleSlope = overlayFrame.RescaleSlope;
						_overlayRescaleIntercept = overlayFrame.RescaleIntercept;

						// compute the bounds of the target overlay image frame in patient coordinates
						var overlayTopLeft = overlayFrame.ImagePlaneHelper.ConvertToPatient(new PointF(0, 0));
						var overlayTopRight = overlayFrame.ImagePlaneHelper.ConvertToPatient(new PointF(overlayFrame.Columns, 0));

						// compute the overlay and base image resolution in pixels per unit patient space (mm).
						var overlayResolution = overlayFrame.Columns/(overlayTopRight - overlayTopLeft).Magnitude;
						var baseResolution = baseFrame.Columns/(baseTopRight - baseTopLeft).Magnitude;

						// compute parameters to register the overlay on the base image
						var scale = baseResolution/overlayResolution;
						var offset = (overlayTopLeft - baseTopLeft)*overlayResolution;

						// validate computed transform parameters
						var overlayBottomLeft = overlayFrame.ImagePlaneHelper.ConvertToPatient(new PointF(0, overlayFrame.Rows));
						float scaleY = baseFrame.Rows*(overlayBottomLeft - overlayTopLeft).Magnitude/(overlayFrame.Rows*(baseBottomLeft - baseTopLeft).Magnitude);
						Platform.CheckTrue(FloatComparer.AreEqual(scale, scaleY), "Computed ScaleX != ScaleY");
						Platform.CheckTrue(offset.Z < 0.5f, "Compute OffsetZ != 0");

						_overlayCoregistrationScale = scale;
						_overlayCoregistrationOffsetX = offset.X;
						_overlayCoregistrationOffsetY = offset.Y;

						return overlayFrame.GetNormalizedPixelData();
					}
				}
			}
		}

		public GrayscaleImageGraphic CreateImageGraphic()
		{
			this.LoadPixelData();
			var imageGraphic = new GrayscaleImageGraphic(
				_overlayRows, _overlayColumns,
				_overlayBitsAllocated, _overlayBitsStored, _overlayHighBit,
				_overlayIsSigned, _overlayIsInverted,
				_overlayRescaleSlope, _overlayRescaleIntercept,
				() => this.OverlayPixelData);
			imageGraphic.SpatialTransform.Scale = _overlayCoregistrationScale;
			imageGraphic.SpatialTransform.TranslationX = _overlayCoregistrationOffsetX;
			imageGraphic.SpatialTransform.TranslationY = _overlayCoregistrationOffsetY;
			return imageGraphic;
		}

		#region Memory Management Support

		private readonly LargeObjectContainerData _largeObjectData = new LargeObjectContainerData(Guid.NewGuid());

		Guid ILargeObjectContainer.Identifier
		{
			get { return _largeObjectData.Identifier; }
		}

		int ILargeObjectContainer.LargeObjectCount
		{
			get { return _largeObjectData.LargeObjectCount; }
		}

		long ILargeObjectContainer.BytesHeldCount
		{
			get { return _largeObjectData.BytesHeldCount; }
		}

		DateTime ILargeObjectContainer.LastAccessTime
		{
			get { return _largeObjectData.LastAccessTime; }
		}

		RegenerationCost ILargeObjectContainer.RegenerationCost
		{
			get { return _largeObjectData.RegenerationCost; }
		}

		bool ILargeObjectContainer.IsLocked
		{
			get { return _largeObjectData.IsLocked; }
		}

		void ILargeObjectContainer.Lock()
		{
			_largeObjectData.Lock();
		}

		void ILargeObjectContainer.Unlock()
		{
			_largeObjectData.Unlock();
		}

		void ILargeObjectContainer.Unload()
		{
			this.UnloadPixelData();
		}

		#endregion
	}
}