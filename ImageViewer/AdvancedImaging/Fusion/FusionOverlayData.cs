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
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.InteractiveGraphics;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Volume.Mpr;
using VolumeData = ClearCanvas.ImageViewer.Volume.Mpr.Volume;

namespace ClearCanvas.ImageViewer.AdvancedImaging.Fusion
{
	public partial class FusionOverlayData : IDisposable, ILargeObjectContainer, IProgressGraphicProgressProvider
	{
		private readonly object _syncVolumeDataLock = new object();
		private readonly object _syncLoaderLock = new object();
		private IList<IFrameReference> _frames;
		private VolumeData _volume;
		private BackgroundTask _volumeLoaderTask;

		public FusionOverlayData(IEnumerable<Frame> overlaySource)
		{
			var frames = new List<IFrameReference>();
			foreach (Frame frame in overlaySource)
				frames.Add(frame.CreateTransientReference());
			_frames = frames.AsReadOnly();
		}

		bool IProgressGraphicProgressProvider.IsRunning(out float progress, out string message)
		{
			return !BeginLoad(out progress, out message);
		}

		public bool BeginLoad(out float progress, out string message)
		{
			// update the last access time
			_largeObjectData.UpdateLastAccessTime();

			// if the data is already available without blocking, return success immediately
			VolumeData volume = _volume;
			if (volume != null)
			{
				progress = 1f;
				message = "Reticulating splines";
				return true;
			}

			lock (_syncLoaderLock)
			{
				progress = 0;
				message = "Watering plants";
				if (_volumeLoaderTask == null)
				{
					// if the data is available now, return success
					volume = _volume;
					if (volume != null)
					{
						progress = 1f;
						message = "Exterminating mites";
						return true;
					}

					_volumeLoaderTask = new BackgroundTask(c => this.LoadVolume(c), false, null);
					_volumeLoaderTask.Run();
					_volumeLoaderTask.Terminated += _volumeLoaderTask_Terminated;
				}
				else
				{
					if (_volumeLoaderTask.LastBackgroundTaskProgress != null)
					{
						message = _volumeLoaderTask.LastBackgroundTaskProgress.Progress.Message;
						progress = _volumeLoaderTask.LastBackgroundTaskProgress.Progress.Percent/100f;
					}
				}
			}
			return false;
		}

		private void _volumeLoaderTask_Terminated(object sender, BackgroundTaskTerminatedEventArgs e)
		{
			if (_volumeLoaderTask != null)
			{
				_volumeLoaderTask.Terminated -= _volumeLoaderTask_Terminated;
				_volumeLoaderTask.Dispose();
				_volumeLoaderTask = null;
			}
		}

		protected VolumeData Volume
		{
			get
			{
				// update the last access time
				_largeObjectData.UpdateLastAccessTime();

				// if the data is already available without blocking, return it immediately
				VolumeData volume = _volume;
				if (volume != null)
					return volume;

				return LoadVolume(null);
			}
		}

		private VolumeData LoadVolume(IBackgroundTaskContext context)
		{
			// wait for synchronized access
			lock (_syncVolumeDataLock)
			{
				// if the data is now available, return it immediately
				// (i.e. we were blocked because we were already reading the data)
				if (_volume != null)
					return _volume;

				// load the volume data
				if (context == null)
					_volume = VolumeData.Create(_frames);
				else
					_volume = VolumeData.Create(_frames, (n, count) =>
					                                     	{
					                                     		context.ReportProgress(new BackgroundTaskProgress(n, count, "Opposumating possums"));

#if DEBUG
					                                     		// artificially inject some more delay so we can see it as it happens
					                                     		// TODO REMOVE THIS REMOVE THIS REMOVE THIS REMOVE THIS REMOVE THIS REMOVE THIS REMOVE THIS REMOVE THIS
					                                     		Thread.Sleep(250);
#endif
					                                     	});

				// update our stats
				_largeObjectData.BytesHeldCount = 2*_volume.SizeInVoxels;
				_largeObjectData.LargeObjectCount = 1;
				_largeObjectData.UpdateLastAccessTime();

				// regenerating the volume data is easy when the source frames are already in memory!
				_largeObjectData.RegenerationCost = RegenerationCost.Low;

				// register with memory manager
				MemoryManager.Add(this);

				return _volume;
			}
		}

		private void UnloadVolume()
		{
			// wait for synchronized access
			lock (_syncVolumeDataLock)
			{
				// dump our data
				if (_volume != null)
				{
					_volume.Dispose();
					_volume = null;
				}

				// update our stats
				_largeObjectData.BytesHeldCount = 0;
				_largeObjectData.LargeObjectCount = 0;

				// unregister with memory manager
				MemoryManager.Remove(this);
			}
		}

		[Obsolete]
		public GrayscaleImageGraphic GetOverlay(Frame baseFrame)
		{
			OverlaySlice overlayFrame;
			byte[] pixelData = GetOverlay(baseFrame, out overlayFrame);
			return overlayFrame.CreateImageGraphic(() => pixelData);
		}

		public byte[] GetOverlay(Frame baseFrame, out OverlaySlice overlaySlice)
		{
			var volume = this.Volume;

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

						overlaySlice = new OverlaySlice(
							overlayFrame.Rows, overlayFrame.Columns,
							overlayFrame.BitsAllocated, overlayFrame.BitsStored,
							overlayFrame.HighBit, overlayFrame.PixelRepresentation != 0 ? true : false,
							overlayFrame.PhotometricInterpretation == PhotometricInterpretation.Monochrome1 ? true : false,
							overlayFrame.RescaleSlope, overlayFrame.RescaleIntercept,
							scale, offset.X, offset.Y);

						return overlayFrame.GetNormalizedPixelData();
					}
				}
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				// this will unload _volume
				this.UnloadVolume();

				if (_frames != null)
				{
					foreach (IFrameReference frame in _frames)
						frame.Dispose();
					_frames = null;
				}
			}
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
			this.UnloadVolume();
		}

		#endregion

		#region OverlaySlice Class

		public class OverlaySlice
		{
			private readonly int _rows, _columns;
			private readonly int _bitsAllocated, _bitsStored, _highBit;
			private readonly bool _isSigned, _isInverted;
			private readonly double _rescaleSlope, _rescaleIntercept;
			private readonly float _coregistrationScale, _coregistrationOffsetX, _coregistrationOffsetY;

			internal OverlaySlice(int rows, int columns,
			                      int bitsAllocated, int bitsStored, int highBit,
			                      bool isSigned, bool isInverted,
			                      double rescaleSlope, double rescaleIntercept,
			                      float scale, float offsetX, float offsetY)
			{
				_rows = rows;
				_columns = columns;
				_bitsAllocated = bitsAllocated;
				_bitsStored = bitsStored;
				_highBit = highBit;
				_isSigned = isSigned;
				_isInverted = isInverted;
				_rescaleSlope = rescaleSlope;
				_rescaleIntercept = rescaleIntercept;
				_coregistrationScale = scale;
				_coregistrationOffsetX = offsetX;
				_coregistrationOffsetY = offsetY;
			}

			public GrayscaleImageGraphic CreateImageGraphic(PixelDataGetter pixelDataGetter)
			{
				var imageGraphic = new GrayscaleImageGraphic(
					_rows, _columns,
					_bitsAllocated, _bitsStored, _highBit,
					_isSigned, _isInverted,
					_rescaleSlope, _rescaleIntercept,
					pixelDataGetter);
				imageGraphic.SpatialTransform.Scale = _coregistrationScale;
				imageGraphic.SpatialTransform.TranslationX = _coregistrationOffsetX;
				imageGraphic.SpatialTransform.TranslationY = _coregistrationOffsetY;
				return imageGraphic;
			}
		}

		#endregion
	}
}