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

using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.PresentationStates.Dicom
{
	[Cloneable(true)]
	public partial class DicomGraphicsPlane : CompositeGraphic
	{
		[CloneIgnore]
		private readonly OverlaysCollection _imageOverlays;

		[CloneIgnore]
		private readonly OverlaysCollection _presentationOverlays;

		[CloneIgnore]
		private ShutterCollection _shutter;

		[CloneIgnore]
		private LayerCollection _layers;

		public DicomGraphicsPlane()
		{
			_imageOverlays = new OverlaysCollection(this);
			_presentationOverlays = new OverlaysCollection(this);

			base.Graphics.Add(_shutter = new ShutterCollection());
			base.Graphics.Add(_layers = new LayerCollection());
		}

		[OnCloneComplete]
		private void OnCloneComplete()
		{
			_shutter = (ShutterCollection) CollectionUtils.SelectFirst(base.Graphics, IsType<ShutterCollection>);
			_layers = (LayerCollection) CollectionUtils.SelectFirst(base.Graphics, IsType<LayerCollection>);

			FillOverlayCollections(_shutter);
			foreach (LayerGraphic layer in _layers)
				FillOverlayCollections(layer.Graphics);
		}

		private void FillOverlayCollections(IEnumerable<IGraphic> collection)
		{
			foreach (OverlayPlaneGraphic overlay in CollectionUtils.Select(collection, IsType<OverlayPlaneGraphic>))
			{
				if (overlay.Source == OverlayPlaneSource.Image)
					_imageOverlays.Add(overlay);
				else if (overlay.Source == OverlayPlaneSource.PresentationState)
					_presentationOverlays.Add(overlay);
				// TODO: handle case for user-created overlays, when we actually support creating such
			}
		}

		public void Clear()
		{
			this.Shutters.Clear();
			this.Layers.Clear();
			this.ImageOverlays.Clear();
			this.PresentationOverlays.Clear();
		}

		/// <summary>
		/// Gets a collection of available shutters.
		/// </summary>
		public IDicomGraphicsPlaneShutters Shutters
		{
			get { return _shutter; }
		}

		/// <summary>
		/// Gets a collection of available graphic layers.
		/// </summary>
		public IDicomGraphicsPlaneLayers Layers
		{
			get { return _layers; }
		}

		/// <summary>
		/// Gets a collection of available overlays from the image SOP.
		/// </summary>
		public IDicomGraphicsPlaneOverlays ImageOverlays
		{
			get { return _imageOverlays; }
		}

		/// <summary>
		/// Gets a collection of available overlays from an associated presentation state SOP, if one exists.
		/// </summary>
		public IDicomGraphicsPlaneOverlays PresentationOverlays
		{
			get { return _presentationOverlays; }
		}

		private static bool IsType<T>(object test)
		{
			return test is T;
		}

		public static DicomGraphicsPlane GetDicomGraphicsPlane(IDicomPresentationImage dicomPresentationImage, bool createIfNecessary)
		{
			if (dicomPresentationImage == null)
				return null;

			GraphicCollection dicomGraphics = dicomPresentationImage.DicomGraphics;
			DicomGraphicsPlane dicomGraphicsPlane = CollectionUtils.SelectFirst(dicomGraphics, IsType<DicomGraphicsPlane>) as DicomGraphicsPlane;

			if (dicomGraphicsPlane == null && createIfNecessary)
				dicomGraphics.Add(dicomGraphicsPlane = new DicomGraphicsPlane());

			return dicomGraphicsPlane;
		}
	}
}