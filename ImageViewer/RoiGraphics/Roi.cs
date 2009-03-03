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
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.RoiGraphics
{
	public abstract class Roi
	{
		private readonly int _imageRows;
		private readonly int _imageColumns;
		private readonly string _modality;
		private readonly PixelData _pixelData;
		private readonly PixelAspectRatio _pixelAspectRatio;
		private readonly PixelSpacing _normalizedPixelSpacing;
		private readonly IComposableLut _modalityLut;

		private RectangleF _boundingBox;

		protected Roi(IPresentationImage presentationImage)
		{
			IImageGraphicProvider provider = presentationImage as IImageGraphicProvider;
			if (provider == null)
				return;

			_imageRows = provider.ImageGraphic.Rows;
			_imageColumns = provider.ImageGraphic.Columns;

			_pixelData = provider.ImageGraphic.PixelData;
			if (presentationImage is IModalityLutProvider)
				_modalityLut = ((IModalityLutProvider) presentationImage).ModalityLut;

			if (presentationImage is IImageSopProvider)
			{
				Frame frame = ((IImageSopProvider) presentationImage).Frame;
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

		public string Modality
		{
			get { return _modality; }
		}

		public RectangleF BoundingBox
		{
			get
			{
				if (_boundingBox.IsEmpty)
					_boundingBox = ComputeBounds();
				return _boundingBox;
			}
		}

		protected abstract RectangleF ComputeBounds();

		public abstract Roi CopyTo(IPresentationImage presentationImage);

		public abstract bool Contains(PointF point);

		public bool Contains(int x, int y)
		{
			return this.Contains(new PointF(x, y));
		}
	}
}