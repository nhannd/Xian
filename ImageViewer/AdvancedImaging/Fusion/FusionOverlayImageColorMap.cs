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
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.AdvancedImaging.Fusion
{
	public class FusionOverlayImageColorMap : ColorMap
	{
		private float _alpha = 1f;
		private float _threshold = 0f;

		public FusionOverlayImageColorMap() {}

		public float Alpha
		{
			get { return _alpha; }
			set
			{
				Platform.CheckTrue(value >= 0 && value <= 1, String.Format("{3} = {0} is invalid. {3} must be >= {1} and <= {2}.", value, 0, 1, "value"));
				if (_alpha != value)
				{
					_alpha = value;
					base.OnLutChanged();
				}
			}
		}

		public float Threshold
		{
			get { return _threshold; }
			set
			{
				Platform.CheckTrue(value >= 0 && value <= 1, String.Format("{3} = {0} is invalid. {3} must be >= {1} and <= {2}.", value, 0, 1, "value"));
				if (_threshold != value)
				{
					_threshold = value;
					base.OnLutChanged();
				}
			}
		}

		/// <summary>
		/// Generates the Lut.
		/// </summary>
		protected override void Create()
		{
			int j = 0;
			int maxGrayLevel = this.Length - 1;
			int min = MinInputValue;
			int max = MaxInputValue;

			for (int i = min; i <= max; i++)
			{
				float scale = j/(float) maxGrayLevel;
				j++;

				if (scale > _threshold)
				{
					int value = (int) (Byte.MaxValue*scale);
					int alpha = (int) (Byte.MaxValue*_alpha);
					this[i] = Color.FromArgb(alpha, value, 0, 0).ToArgb();
				}
				else
				{
					this[i] = 0;
				}
			}
		}

		/// <summary>
		/// Returns an abbreviated description of the Lut.
		/// </summary>
		public override string GetDescription()
		{
			return "YARRRRRRRRRRGB";
		}
	}
}