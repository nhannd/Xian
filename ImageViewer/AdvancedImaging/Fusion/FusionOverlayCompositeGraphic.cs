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
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.InteractiveGraphics;

namespace ClearCanvas.ImageViewer.AdvancedImaging.Fusion
{
	[Cloneable(false)]
	internal partial class FusionOverlayCompositeGraphic : CompositeGraphic, IVoiLutProvider, IColorMapProvider, ILayerOpacityProvider
	{
		[CloneIgnore]
		private IFusionOverlayFrameDataReference _overlayFrameDataReference;

		[CloneIgnore]
		private GrayscaleImageGraphic _overlayImageGraphic;

		private VoiLutManagerProxy _voiLutManagerProxy;
		private ColorMapManagerProxy _colorMapManagerProxy;

		public FusionOverlayCompositeGraphic(FusionOverlayFrameData overlayFrameData)
		{
			_overlayFrameDataReference = overlayFrameData.CreateTransientReference();
			_overlayFrameDataReference.FusionOverlayFrameData.Unloaded += HandleOverlayFrameDataUnloaded;
			_voiLutManagerProxy = new VoiLutManagerProxy();
			_colorMapManagerProxy = new ColorMapManagerProxy();
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		/// <param name="source">The source object from which to clone.</param>
		/// <param name="context">The cloning context object.</param>
		protected FusionOverlayCompositeGraphic(FusionOverlayCompositeGraphic source, ICloningContext context)
		{
			context.CloneFields(source, this);

			_overlayFrameDataReference = source._overlayFrameDataReference.Clone();
			_overlayFrameDataReference.FusionOverlayFrameData.Unloaded += HandleOverlayFrameDataUnloaded;
		}

		[OnCloneComplete]
		private void OnCloneComplete()
		{
			_overlayImageGraphic = (GrayscaleImageGraphic) CollectionUtils.SelectFirst(base.Graphics, g => g is GrayscaleImageGraphic);
			if (_overlayImageGraphic != null)
			{
				_voiLutManagerProxy.SetRealVoiLutManager(_overlayImageGraphic.VoiLutManager);
				_colorMapManagerProxy.SetRealColorMapManager(_overlayImageGraphic.ColorMapManager);
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				_overlayImageGraphic = null;
				_voiLutManagerProxy = null;
				_colorMapManagerProxy = null;

				if (_overlayFrameDataReference != null)
				{
					_overlayFrameDataReference.FusionOverlayFrameData.Unloaded -= HandleOverlayFrameDataUnloaded;
					_overlayFrameDataReference.Dispose();
					_overlayFrameDataReference = null;
				}
			}

			base.Dispose(disposing);
		}

		public IVoiLutManager VoiLutManager
		{
			get { return _voiLutManagerProxy; }
		}

		public IColorMapManager ColorMapManager
		{
			get { return _colorMapManagerProxy; }
		}

		public ILayerOpacityManager LayerOpacityManager
		{
			get { return _colorMapManagerProxy; }
		}

		public FusionOverlayFrameData OverlayFrameData
		{
			get { return _overlayFrameDataReference.FusionOverlayFrameData; }
		}

		public GrayscaleImageGraphic OverlayImageGraphic
		{
			get { return _overlayImageGraphic; }
			private set
			{
				if (_overlayImageGraphic != value)
				{
					if (_overlayImageGraphic != null)
					{
						base.Graphics.Remove(_overlayImageGraphic);
						_voiLutManagerProxy.SetRealVoiLutManager(null);
						_colorMapManagerProxy.SetRealColorMapManager(null);

						// disposal must be last so that the proxy objects have a chance to grab a memento in case we need to reload it later
						_overlayImageGraphic.Dispose();
					}

					_overlayImageGraphic = value;

					if (_overlayImageGraphic != null)
					{
						_voiLutManagerProxy.SetRealVoiLutManager(_overlayImageGraphic.VoiLutManager);
						_colorMapManagerProxy.SetRealColorMapManager(_overlayImageGraphic.ColorMapManager);
						base.Graphics.Insert(0, _overlayImageGraphic);
					}
				}
			}
		}

		public override void OnDrawing()
		{
			if (_overlayImageGraphic == null)
			{
				_overlayFrameDataReference.FusionOverlayFrameData.Lock();
				try
				{
					if (this.ParentPresentationImage == null || !this.ParentPresentationImage.Visible)
					{
						// we're drawing to an offscreen buffer, so force the frame data to load synchronously now (progress bars must be visible to be useful)
						_overlayFrameDataReference.FusionOverlayFrameData.Load();
					}

					var progressGraphic = (ProgressGraphic) CollectionUtils.SelectFirst(this.Graphics, g => g is ProgressGraphic);

					float progress;
					string message;
					if (_overlayFrameDataReference.FusionOverlayFrameData.BeginLoad(out progress, out message))
					{
						OverlayImageGraphic = _overlayFrameDataReference.FusionOverlayFrameData.CreateImageGraphic();

#if DEBUG
						if (this.OverlayFrameData.BaseFrame.FrameOfReferenceUid != this.OverlayFrameData.OverlayFrameOfReferenceUid)
						{
							if (!CollectionUtils.Contains(base.Graphics, g => g is CenteredTextGraphic))
								this.Graphics.Add(new CenteredTextGraphic("Frame of Reference (0020,0052) MISMATCH"));
						}
#endif

						if (progressGraphic != null)
						{
							this.Graphics.Remove(progressGraphic);
							progressGraphic.Dispose();
						}
					}
					else if (progressGraphic == null)
					{
						this.Graphics.Add(new ProgressGraphic(_overlayFrameDataReference.FusionOverlayFrameData, true, ProgressBarGraphicStyle.Continuous));
					}
				}
				finally
				{
					_overlayFrameDataReference.FusionOverlayFrameData.Unlock();
				}
			}
			base.OnDrawing();
		}

		private void HandleOverlayFrameDataUnloaded(object sender, EventArgs e)
		{
			OverlayImageGraphic = null;
		}

		public GrayscaleImageGraphic CreateStaticOverlayImageGraphic(bool forceLoad)
		{
			_overlayFrameDataReference.FusionOverlayFrameData.Lock();
			try
			{
				if (!_overlayFrameDataReference.FusionOverlayFrameData.IsLoaded)
				{
					if (!forceLoad)
						return null;

					_overlayFrameDataReference.FusionOverlayFrameData.Load();
				}

				if (OverlayImageGraphic == null)
					OverlayImageGraphic = _overlayFrameDataReference.FusionOverlayFrameData.CreateImageGraphic();

				var staticClone = new GrayscaleImageGraphic(
					OverlayImageGraphic.Rows, OverlayImageGraphic.Columns,
					OverlayImageGraphic.BitsPerPixel, OverlayImageGraphic.BitsStored, OverlayImageGraphic.HighBit,
					OverlayImageGraphic.IsSigned, OverlayImageGraphic.Invert,
					OverlayImageGraphic.RescaleSlope, OverlayImageGraphic.RescaleIntercept,
					OverlayImageGraphic.PixelData.Raw);
				staticClone.VoiLutManager.SetMemento(OverlayImageGraphic.VoiLutManager.CreateMemento());
				staticClone.ColorMapManager.SetMemento(OverlayImageGraphic.ColorMapManager.CreateMemento());
				staticClone.SpatialTransform.SetMemento(OverlayImageGraphic.SpatialTransform.CreateMemento());
				return staticClone;
			}
			finally
			{
				_overlayFrameDataReference.FusionOverlayFrameData.Unlock();
			}
		}

		#region CenteredTextGraphic Class

#if DEBUG
		[Cloneable(true)]
		private class CenteredTextGraphic : InvariantTextPrimitive
		{
			public CenteredTextGraphic(string text) : base(text) {}

			/// <summary>
			/// Cloning constructor.
			/// </summary>
			private CenteredTextGraphic() : base() {}

			public override void OnDrawing()
			{
				if (ParentPresentationImage != null)
				{
					CoordinateSystem = CoordinateSystem.Destination;
					try
					{
						var rectangle = ParentPresentationImage.ClientRectangle;
						Location = new PointF(rectangle.Width/2f, rectangle.Height/2f);
					}
					finally
					{
						ResetCoordinateSystem();
					}
				}
				base.OnDrawing();
			}
		}
#endif

		#endregion
	}
}