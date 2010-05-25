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
		private ColorBarGraphic _colorBarGraphic;

		[CloneIgnore]
		private GrayscaleImageGraphic _fusionOverlayImageGraphic;

		private FusionVoiLutManagerProxy _voiLutManagerProxy;
		private FusionOverlayColorMapSpec _overlayColorMapSpec;

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
				_colorBarGraphic = (ColorBarGraphic) CollectionUtils.SelectFirst(_fusionOverlayLayer.Graphics, g => g is ColorBarGraphic);
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

			if (_colorBarGraphic == null)
			{
				_colorBarGraphic = new ColorBarGraphic();
				_fusionOverlayLayer.Graphics.Add(_colorBarGraphic);
			}

			if (_voiLutManagerProxy == null)
			{
				_voiLutManagerProxy = new FusionVoiLutManagerProxy();
			}
			_voiLutManagerProxy.SetBaseVoiLutManager(this.ImageGraphic.VoiLutManager);

			if (_overlayColorMapSpec == null)
			{
				_overlayColorMapSpec = new FusionOverlayColorMapSpec();
			}
			_overlayColorMapSpec.SetColorBarColorMapManager(_colorBarGraphic.ColorMapManager);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				_fusionOverlayImageGraphic = null;
				_colorBarGraphic = null;
				_fusionOverlayLayer = null;

				if (_overlayDataReference != null)
				{
					_overlayDataReference.Dispose();
					_overlayDataReference = null;
				}

				if (_voiLutManagerProxy != null)
				{
					_voiLutManagerProxy.Dispose();
					_voiLutManagerProxy = null;
				}

				if (_overlayColorMapSpec != null)
				{
					_overlayColorMapSpec.Dispose();
					_overlayColorMapSpec = null;
				}
			}

			base.Dispose(disposing);
		}

		public FusionPresentationImageLayer ActiveLayer
		{
			get { return _voiLutManagerProxy.ActiveLayer; }
			set { _voiLutManagerProxy.ActiveLayer = value; }
		}

		public StandardColorMaps OverlayColorMap
		{
			get { return _overlayColorMapSpec.ColorMap; }
			set { _overlayColorMapSpec.ColorMap = value; }
		}

		public float OverlayOpacity
		{
			get { return _overlayColorMapSpec.Opacity; }
			set { _overlayColorMapSpec.Opacity = value; }
		}

		public bool HideOverlayBackground
		{
			get { return _overlayColorMapSpec.HideBackground; }
			set { _overlayColorMapSpec.HideBackground = value; }
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
				_voiLutManagerProxy.SetOverlayVoiLutManager(overlayImageGraphic.VoiLutManager);
				_overlayColorMapSpec.SetOverlayColorMapManager(overlayImageGraphic.ColorMapManager);
				_fusionOverlayLayer.Graphics.Add(_fusionOverlayImageGraphic = overlayImageGraphic);
			}
		}

		#region IVoiLutProvider Members

		IVoiLutManager IVoiLutProvider.VoiLutManager
		{
			get { return _voiLutManagerProxy; }
		}

		#endregion
	}
}