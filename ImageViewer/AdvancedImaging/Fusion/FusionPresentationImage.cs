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
using ClearCanvas.ImageViewer.InteractiveGraphics;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.AdvancedImaging.Fusion
{
	[Cloneable]
	public class FusionPresentationImage : DicomGrayscalePresentationImage, IVoiLutProvider, IImageSopProvider
	{
		private const string _fusionOverlayLayerName = "Fusion";

		[CloneIgnore]
		private AsynchronousLoaderCompositeGraphic _loaderComposite;

		[CloneIgnore]
		private CompositeGraphic _fusionOverlayLayer;

		[CloneIgnore]
		private ColorBarGraphic _colorBarGraphic;

		private FusionVoiLutManagerProxy _voiLutManagerProxy;
		private FusionOverlayColorMapSpec _overlayColorMapSpec;

		public FusionPresentationImage(Frame baseFrame, FusionOverlaySlice overlayData)
			: this(baseFrame.CreateTransientReference(), overlayData.CreateTransientReference()) {}

		public FusionPresentationImage(IFrameReference baseFrame, IFusionOverlaySliceReference overlaySlice)
			: base(baseFrame)
		{
			_loaderComposite = new AsynchronousLoaderCompositeGraphic(overlaySlice.FusionOverlaySlice);

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
		}

		[OnCloneComplete]
		private void OnCloneComplete()
		{
			_fusionOverlayLayer = (CompositeGraphic) CollectionUtils.SelectFirst(base.CompositeImageGraphic.Graphics,
			                                                                     g => g is CompositeGraphic && g.Name == _fusionOverlayLayerName);

			if (_fusionOverlayLayer != null)
			{
				_loaderComposite = (AsynchronousLoaderCompositeGraphic)CollectionUtils.SelectFirst(_fusionOverlayLayer.Graphics, g => g is AsynchronousLoaderCompositeGraphic);
				_colorBarGraphic = (ColorBarGraphic) CollectionUtils.SelectFirst(_fusionOverlayLayer.Graphics, g => g is ColorBarGraphic);
			}

			Initialize();
		}

		private void Initialize()
		{
			if (_fusionOverlayLayer == null)
			{
				_fusionOverlayLayer = new CompositeGraphic {Name = _fusionOverlayLayerName};
				_fusionOverlayLayer.Graphics.Add(_loaderComposite);

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
				_loaderComposite = null;
				_colorBarGraphic = null;
				_fusionOverlayLayer = null;

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
			return new FusionPresentationImage(this.Frame, _loaderComposite.OverlaySlice) {PresentationState = this.PresentationState};
		}

		#region IVoiLutProvider Members

		IVoiLutManager IVoiLutProvider.VoiLutManager
		{
			get { return _voiLutManagerProxy; }
		}

		#endregion
	}
}