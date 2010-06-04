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
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InteractiveGraphics;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.AdvancedImaging.Fusion
{
	public partial class FusionOverlayFrameData : IDisposable, ILargeObjectContainer, IProgressGraphicProgressProvider
	{
		private readonly object _syncPixelDataLock = new object();
		private IFrameReference _baseFrameReference;
		private IFusionOverlayDataReference _overlayDataReference;

		private FusionOverlayData.OverlayFrameParams _overlayFrameParams;
		private byte[] _overlayPixelData;

		internal FusionOverlayFrameData(IFrameReference baseFrame, IFusionOverlayDataReference overlayData)
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

		public FusionOverlayData OverlayData
		{
			get { return _overlayDataReference.FusionOverlayData; }
		}

		bool IProgressGraphicProgressProvider.IsRunning(out float progress, out string message)
		{
			return !BeginLoad(out progress, out message);
		}

		public bool BeginLoad(out float progress, out string message)
		{
			return _overlayDataReference.FusionOverlayData.BeginLoad(out progress, out message);
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
				_overlayPixelData = _overlayDataReference.FusionOverlayData.GetOverlay(_baseFrameReference.Frame, out _overlayFrameParams);

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

		public GrayscaleImageGraphic CreateImageGraphic()
		{
			this.LoadPixelData();
			return new FusionOverlayImageGraphic(this);
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

		#region FusionOverlayImageGraphic Class

		private byte[] GetPixelData()
		{
			return this.OverlayPixelData;
		}

		[Cloneable]
		private class FusionOverlayImageGraphic : GrayscaleImageGraphic
		{
			[CloneIgnore]
			private IFusionOverlayFrameDataReference _overlayFrameData;

			private bool _firstDraw;

			public FusionOverlayImageGraphic(FusionOverlayFrameData fusionOverlayFrameData)
				: base(fusionOverlayFrameData._overlayFrameParams.Rows, fusionOverlayFrameData._overlayFrameParams.Columns,
				       fusionOverlayFrameData._overlayFrameParams.BitsAllocated, fusionOverlayFrameData._overlayFrameParams.BitsStored, fusionOverlayFrameData._overlayFrameParams.HighBit,
				       fusionOverlayFrameData._overlayFrameParams.IsSigned, fusionOverlayFrameData._overlayFrameParams.IsInverted,
				       fusionOverlayFrameData._overlayFrameParams.RescaleSlope, fusionOverlayFrameData._overlayFrameParams.RescaleIntercept,
				       fusionOverlayFrameData.GetPixelData)
			{
				// this image graphic needs to keep a transient reference on the slice, otherwise it could get disposed before we do!
				_overlayFrameData = fusionOverlayFrameData.CreateTransientReference();
				_firstDraw = true;
			}

			/// <summary>
			/// Cloning constructor.
			/// </summary>
			/// <param name="source">The source object from which to clone.</param>
			/// <param name="context">The cloning context object.</param>
			protected FusionOverlayImageGraphic(FusionOverlayImageGraphic source, ICloningContext context) : base(source, context)
			{
				context.CloneFields(source, this);

				_overlayFrameData = source._overlayFrameData.Clone();
			}

			protected override void Dispose(bool disposing)
			{
				if (disposing)
				{
					if (_overlayFrameData != null)
					{
						_overlayFrameData.Dispose();
						_overlayFrameData = null;
					}
				}

				base.Dispose(disposing);
			}

			public override void OnDrawing()
			{
				if (_firstDraw)
				{
					var spatialTransform = this.SpatialTransform;
					if (spatialTransform != null)
					{
						var overlayFrameParams = _overlayFrameData.FusionOverlayFrameData._overlayFrameParams;
						spatialTransform.Scale = overlayFrameParams.CoregistrationScale;
						spatialTransform.TranslationX = overlayFrameParams.CoregistrationOffsetX;
						spatialTransform.TranslationY = overlayFrameParams.CoregistrationOffsetY;
					}
					_firstDraw = false;
				}
				base.OnDrawing();
			}
		}

		#endregion
	}
}