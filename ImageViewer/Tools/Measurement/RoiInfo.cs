#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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

using System.Drawing;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.InteractiveGraphics;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Tools.Measurement;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	public abstract class RoiInfo
	{
		private RoiAnalysisMode _mode;

		private int _imageRows;
		private int _imageColumns;
		private string _modality;
		private PixelData _pixelData;
		private PixelAspectRatio _pixelAspectRatio;
		private PixelSpacing _normalizedPixelSpacing;
		private IComposableLut _modalityLut;
		private RectangleF _boundingBox;

		protected RoiInfo()
		{
		}

		/// <summary>
		/// Set by the <see cref="MeasurementTool{T}"/>.
		/// </summary>
		public RoiAnalysisMode Mode
		{
			get { return _mode; }
			internal set { _mode = value; }
		}

		public int ImageRows
		{
			get { return _imageRows; }
		}

		public int ImageColumns
		{
			get { return _imageColumns; }
		}

		public PixelData PixelData
		{
			get { return _pixelData; }
		}

		public PixelAspectRatio PixelAspectRatio
		{
			get { return _pixelAspectRatio; }
		}

		public PixelSpacing NormalizedPixelSpacing
		{
			get { return _normalizedPixelSpacing; }
		}

		public IComposableLut ModalityLut
		{
			get { return _modalityLut; }
		}

		public RectangleF BoundingBox
		{
			get { return _boundingBox; }
		}

		public string Modality
		{
			get { return _modality; }
		}

		public virtual bool IsValid()
		{
			return this.PixelData != null;
		}

		/// <summary>
		/// Convenience method for initializing a <see cref="RoiInfo"/> object
		/// from an <see cref="InteractiveGraphic"/>.
		/// </summary>
		protected internal virtual void Initialize(InteractiveGraphic roi)
		{
			IPresentationImage image = roi.ParentPresentationImage;
			IImageGraphicProvider provider = image as IImageGraphicProvider;
			if (provider == null)
				return;

			_imageRows = provider.ImageGraphic.Rows;
			_imageColumns = provider.ImageGraphic.Columns;

			_pixelData = provider.ImageGraphic.PixelData;
			if (image is IModalityLutProvider)
				_modalityLut = ((IModalityLutProvider)image).ModalityLut;

			roi.CoordinateSystem = CoordinateSystem.Source;
			_boundingBox = roi.BoundingBox;
			roi.ResetCoordinateSystem();

			if (image is IImageSopProvider)
			{
				Frame frame = ((IImageSopProvider)image).Frame;
				_normalizedPixelSpacing = frame.NormalizedPixelSpacing;
				_pixelAspectRatio = frame.PixelAspectRatio;
				_modality = frame.ParentImageSop.Modality;
			}
			else
			{
				_normalizedPixelSpacing = new PixelSpacing(0, 0);
				_pixelAspectRatio = new PixelAspectRatio(0, 0);
			}
		}
	}
}