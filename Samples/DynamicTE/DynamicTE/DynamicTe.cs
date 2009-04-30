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

using System;
using System.Drawing;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.Tools.ImageProcessing.DynamicTe
{
	public class DynamicTe : IMemorable
	{
		public class DynamicTeMemento : IEquatable<DynamicTeMemento>
		{
			private double _te;
			private int _threshold;
			private int _opacity;

			public DynamicTeMemento(double te, int threshold, int opacity)
			{
				_te = te;
				_threshold = threshold;
				_opacity = opacity;
			}

			public double Te
			{
				get { return _te; }
			}

			public int Threshold
			{
				get { return _threshold; }
			}

			public int Opacity
			{
				get { return _opacity; }
			}

			public override int GetHashCode()
			{
				return base.GetHashCode();
			}

			public override bool Equals(object obj)
			{
				if (obj == this)
					return true;

				return this.Equals(obj as DynamicTeMemento);
			}

			#region IEquatable<DynamicTeMemento> Members

			public bool Equals(DynamicTeMemento other)
			{
				if (other == null)
					return false;

				return Te == other.Te && Threshold == other.Threshold && Opacity == other.Opacity;
			}

			#endregion
		}

		private GrayscaleImageGraphic _imageGraphic;
		private ColorImageGraphic _probabilityOverlay;
		private double _te;
		private byte[] _protonDensityMap;
		private byte[] _t2Map;
		private byte[] _probabilityMap;
		private int _threshold;
		private int _opacity;

		public DynamicTe(
			GrayscaleImageGraphic imageGraphic,
			byte[] protonDensityMap,
			byte[] t2Map,
			ColorImageGraphic probabilityOverlay,
			byte[] probabilityMap)
		{
			_imageGraphic = imageGraphic;
			_protonDensityMap = protonDensityMap;
			_t2Map = t2Map;

			_probabilityOverlay = probabilityOverlay;
			_probabilityMap = probabilityMap;
		}

		internal byte[] ProtonDensityMap
		{
			get { return _protonDensityMap; }
		}

		internal byte[] T2Map
		{
			get { return _t2Map; }
		}

		internal byte[] ProbabilityMap
		{
			get { return _probabilityMap; }
		}

		public double Te
		{
			get { return _te; }
			set
			{
				if (value != _te)
				{
					if (value < 0.0)
						_te = 0.0;
					else
						_te = value;

					CalculatePixelData();
				}
			}
		}

		public bool ProbabilityMapVisible
		{
			get { return _probabilityOverlay.Visible; }
			set { _probabilityOverlay.Visible = value; }
		}

		public void ApplyProbabilityThreshold(int threshold, int opacity)
		{
			if (!this.ProbabilityMapVisible)
				return;

			_threshold = threshold;
			_opacity = opacity;

			GrayscalePixelData probabilityPixelData = new GrayscalePixelData(
				_imageGraphic.Rows,
				_imageGraphic.Columns,
				_imageGraphic.BitsPerPixel,
				_imageGraphic.BitsStored,
				_imageGraphic.HighBit,
				_imageGraphic.IsSigned,
				_probabilityMap);

			probabilityPixelData.ForEachPixel(
				delegate(int i, int x, int y, int pixelIndex)
					{
						if (probabilityPixelData.GetPixel(pixelIndex) < threshold)
							_probabilityOverlay.PixelData.SetPixel(pixelIndex, Color.FromArgb(opacity, Color.Red));
						else
							_probabilityOverlay.PixelData.SetPixel(pixelIndex, Color.Empty);
					});
		}

		private unsafe void CalculatePixelData()
		{
			byte[] pixelData = _imageGraphic.PixelData.Raw;
			int sizeInPixels = _imageGraphic.SizeInPixels;

			fixed (byte* pPixelData = pixelData)
			{
				fixed (byte* pProtonDensityMap = _protonDensityMap)
				{
					fixed (byte* pT2Map = _t2Map)
					{
						if (_imageGraphic.IsSigned)
						{
							for (int i = 0; i < sizeInPixels; i++)
							{
								short* pShortPixelData = (short*)pPixelData;
								short* pShortProtonDensityMap = (short*)pProtonDensityMap;
								short* pShortT2Map = (short*)pT2Map;

								short pixelValue = (short)(pShortProtonDensityMap[i] * Math.Exp(-this.Te / pShortT2Map[i]));
								pShortPixelData[i] = pixelValue;
							}
						}
						else
						{
							for (int i = 0; i < sizeInPixels; i++)
							{
								ushort* pShortPixelData = (ushort*)pPixelData;
								ushort* pShortProtonDensityMap = (ushort*)pProtonDensityMap;
								ushort* pShortT2Map = (ushort*)pT2Map;

								ushort pixelValue = (ushort)(pShortProtonDensityMap[i] * Math.Exp(-this.Te / pShortT2Map[i]));
								pShortPixelData[i] = pixelValue;
							}
						}
					}
				}
			}
		}

		#region IMemorable Members

		public object CreateMemento()
		{
			return new DynamicTeMemento(this.Te, _threshold, _opacity);
		}

		public void SetMemento(object memento)
		{
			DynamicTeMemento teMemento = (DynamicTeMemento) memento;

			this.Te = teMemento.Te;
			ApplyProbabilityThreshold(teMemento.Threshold, teMemento.Opacity);
			_imageGraphic.Draw();
		}

		#endregion
	}
}
