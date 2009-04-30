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

using ClearCanvas.ImageViewer.Graphics;
using System.Drawing;
using ClearCanvas.Desktop;
using ClearCanvas.Common;

namespace ClearCanvas.Samples.Cad
{
	public class CadOverlayGraphic : ColorImageGraphic, IMemorable
	{
		public class CadMemento : IMemento
		{
			private int _threshold;
			private int _opacity;

			public CadMemento(int threshold, int opacity)
			{
				_threshold = threshold;
				_opacity = opacity;
			}

			public int Threshold
			{
				get { return _threshold; }
			}

			public int Opacity
			{
				get { return _opacity; }
			}
		}

		private int _threshold;
		private int _opacity;
		private GrayscaleImageGraphic _image;

		public CadOverlayGraphic(GrayscaleImageGraphic image) 
			: base(image.Rows, image.Columns)
		{
			_image = image;		
		}

		public int Threshold
		{
			get { return _threshold; }
			set { _threshold = value; }
		}

		public int Opacity
		{
			get { return _opacity; }
			set { _opacity = value; }
		}

		private Color OverlayColor
		{
			get
			{
				int alpha = (int)(this.Opacity / 100.0f * 255);
				return Color.FromArgb(alpha, Color.Red);
			}
		}

		public void Analyze()
		{
			this.PixelData.ForEachPixel(
				delegate(int i, int x, int y, int pixelIndex)
					{
						int pixelValue = _image.PixelData.GetPixel(pixelIndex);
						int hounsfieldValue = _image.ModalityLut[pixelValue];
						if (hounsfieldValue > this.Threshold)
							this.PixelData.SetPixel(pixelIndex, this.OverlayColor);
						else
							this.PixelData.SetPixel(pixelIndex, Color.Empty);
					});

			Draw();
		}

		#region IMemorable Members

		public IMemento CreateMemento()
		{
			return new CadMemento(_threshold, _opacity);
		}

		public void SetMemento(IMemento memento)
		{
			CadMemento cadMemento = memento as CadMemento;
			Platform.CheckForInvalidCast(cadMemento, "memento", "CadMemento");

			_threshold = cadMemento.Threshold;
			_opacity = cadMemento.Opacity;
			Analyze();
		}

		#endregion
	}
}