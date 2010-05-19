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

using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.AdvancedImaging.Fusion
{
	[Cloneable]
	public class FusionPresentationImage : DicomGrayscalePresentationImage, IVoiLutProvider, IImageSopProvider
	{
		private const string _fusionOverlayLayerName = "Fusion";

		[CloneIgnore]
		private IFusionOverlayDataReference _overlayDataReference;

		[CloneIgnore]
		private CompositeGraphic _fusionOverlayLayer;

		[CloneIgnore]
		private GrayscaleImageGraphic _fusionOverlayImageGraphic;

		private float _colorMapAlpha = 0.5f;
		private float _colorMapThreshold = 0.1f;
		private FusionPresentationImageLayer _activeLayer = FusionPresentationImageLayer.Base;

		public FusionPresentationImage(Frame baseFrame, FusionOverlayData overlayData)
			: this(baseFrame.CreateTransientReference(), overlayData.CreateTransientReference()) {}

		public FusionPresentationImage(IFrameReference baseFrame, IFusionOverlayDataReference overlayData)
			: base(baseFrame)
		{
			_overlayDataReference = overlayData;

			Initialize();
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		/// <param name="source">The source object from which to clone.</param>
		/// <param name="context">The cloning context object.</param>
		protected FusionPresentationImage(FusionPresentationImage source, ICloningContext context) : base(source, context)
		{
			context.CloneFields(source, this);

			_overlayDataReference = source._overlayDataReference.Clone();
		}

		[OnCloneComplete]
		private void OnCloneComplete()
		{
			_fusionOverlayLayer = (CompositeGraphic) CollectionUtils.SelectFirst(base.CompositeImageGraphic.Graphics,
			                                                                     g => g is CompositeGraphic && g.Name == _fusionOverlayLayerName);

			if (_fusionOverlayLayer != null)
			{
				_fusionOverlayImageGraphic = (GrayscaleImageGraphic) CollectionUtils.SelectFirst(_fusionOverlayLayer.Graphics, g => g is GrayscaleImageGraphic);
			}

			Initialize();
		}

		private void Initialize()
		{
			if (_fusionOverlayLayer == null)
			{
				_fusionOverlayLayer = new CompositeGraphic {Name = _fusionOverlayLayerName};

				// insert the fusion graphics layer right after the base image graphic (both contain domain-level graphics)
				base.CompositeImageGraphic.Graphics.Insert(base.CompositeImageGraphic.Graphics.IndexOf(this.ImageGraphic) + 1, _fusionOverlayLayer);
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_overlayDataReference != null)
				{
					_overlayDataReference.Dispose();
					_overlayDataReference = null;
				}

				if (_fusionOverlayImageGraphic != null)
				{
					// do not dispose this - we don't own it directly!
					_fusionOverlayImageGraphic = null;
				}

				if (_fusionOverlayLayer != null)
				{
					// do not dispose this - we don't own it directly!
					_fusionOverlayLayer = null;
				}
			}

			base.Dispose(disposing);
		}

		public FusionPresentationImageLayer ActiveLayer
		{
			get { return _activeLayer; }
			set
			{
				if (_activeLayer != value)
				{
					_activeLayer = value;
				}
			}
		}

		public float OverlayAlpha
		{
			get { return _colorMapAlpha; }
			set
			{
				if (_colorMapAlpha != value)
				{
					_colorMapAlpha = value;
				}
			}
		}

		public override IPresentationImage CreateFreshCopy()
		{
			return new FusionPresentationImage(this.Frame, _overlayDataReference.FusionOverlayData) {PresentationState = this.PresentationState};
		}

		protected override void OnDrawing()
		{
			if (_fusionOverlayImageGraphic == null)
			{
				//BackgroundTask bt = new BackgroundTask(this.GetOverlayImageGraphic, false);
				//ProgressGraphic.Draw(null, this, false, ProgressBarStyle.Blocks);

				GetOverlayImageGraphic(null);
			}

			base.OnDrawing();
		}

		private void GetOverlayImageGraphic(IBackgroundTaskContext context)
		{
			var overlayImageGraphic = _overlayDataReference.FusionOverlayData.GetOverlay(this.Frame);
			if (overlayImageGraphic != null)
			{
				overlayImageGraphic.ColorMapManager.InstallColorMap("HotMetal");
				_fusionOverlayLayer.Graphics.Add(_fusionOverlayImageGraphic = overlayImageGraphic);
			}
		}

		#region IVoiLutProvider Members

		IVoiLutManager IVoiLutProvider.VoiLutManager
		{
			get
			{
				if (_activeLayer == FusionPresentationImageLayer.Overlay)
				{
					if (_fusionOverlayImageGraphic != null)
						return _fusionOverlayImageGraphic.VoiLutManager;
				}
				else
				{
					return base.VoiLutManager;
				}
				return base.VoiLutManager;
			}
		}

		#endregion
	}
}