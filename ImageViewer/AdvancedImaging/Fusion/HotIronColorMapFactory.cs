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

using System.Drawing;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.AdvancedImaging.Fusion
{
	public class HotIronColorMapFactory : IColorMapFactory
	{
		public static readonly string ColorMapName = "HOT_IRON";
		private const string _colorMapDescription = "Hot Iron";

		public string Name
		{
			get { return ColorMapName; }
		}

		public string Description
		{
			get { return _colorMapDescription; }
		}

		public IDataLut Create()
		{
			return new DicomPaletteColorMap(_colorMapDescription, PaletteColorLut.HotIron);
		}

		private class DicomPaletteColorMap : ColorMap
		{
			private readonly PaletteColorLut _paletteColorLut;
			private readonly string _description;

			public DicomPaletteColorMap(PaletteColorLut paletteColorLut)
				: this(string.Empty, paletteColorLut) {}

			public DicomPaletteColorMap(string description, PaletteColorLut paletteColorLut)
			{
				_description = description;
				_paletteColorLut = paletteColorLut;
			}

			protected override void Create()
			{
				int valueIndex = 0;
				int valueFullScale = this.Length - 1;
				int min = MinInputValue;
				int max = MaxInputValue;

				int countColors = _paletteColorLut.Data.Length;
				for (int i = min; i <= max; i++)
				{
					// scale the input value index to the range of colors available
					float scaledColorIndex = (countColors - 1f)*(valueIndex++)/valueFullScale;

					// the fractional part of the scaled index is the interpolation step between two adjacent colors
					int colorIndex = (int) scaledColorIndex;
					float interpolationStep = scaledColorIndex - colorIndex;

					// if the index is within the LUT's bounds, interpolate between the adjacent colors
					if (colorIndex + 1 < countColors)
					{
						Color startColor = _paletteColorLut.Data[colorIndex];
						Color endColor = _paletteColorLut.Data[colorIndex + 1];
						Color color = Color.FromArgb(255,
						                             Interpolate(interpolationStep, startColor.R, endColor.R),
						                             Interpolate(interpolationStep, startColor.G, endColor.G),
						                             Interpolate(interpolationStep, startColor.B, endColor.B));
						this[i] = color.ToArgb();
					}
					else
					{
						// otherwise, just return the last value (should only happen once, i.e. valueIndex==valueFullScale)
						this[i] = _paletteColorLut.Data[countColors - 1].ToArgb();
					}
				}
			}

			private static int Interpolate(float value, byte low, byte high)
			{
				return 0x000000FF & (int) (value*(high - low) + low);
			}

			public override string GetKey()
			{
				return string.Format("{0}[{1}]", base.GetKey(), _paletteColorLut.SourceSopInstanceUid);
			}

			public override string GetDescription()
			{
				return _description;
			}
		}
	}
}