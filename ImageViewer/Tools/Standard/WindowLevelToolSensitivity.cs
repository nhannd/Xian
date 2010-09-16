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

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	public partial class WindowLevelTool
	{
		private static readonly SensitivityMap Sensitivity = new SensitivityMap();

		private double CurrentSensitivity
		{
			get
			{
				if (SelectedVoiLutProvider != null)
				{
					var voiLutManager = SelectedVoiLutProvider.VoiLutManager;
					if (voiLutManager != null && voiLutManager.VoiLut != null)
					{
						// compute the effective bit depth of the image (that is, the bit depth of values on which the W/L tool operates, rather than that of the raw pixel data)
						// the formula to compute this is: ceil(log2(EFFECTIVE_VALUE_RANGE))
						var effectiveBitDepth = (int) Math.Ceiling(Math.Log(Math.Abs(voiLutManager.VoiLut.MaxOutputValue - voiLutManager.VoiLut.MinOutputValue), 2));
						return Sensitivity[effectiveBitDepth];
					}
				}
				return 10;
			}
		}

		private class SensitivityMap
		{
			private static readonly double[] _increment;

			static SensitivityMap()
			{
				_increment = new double[16];
				_increment[0] = 0.05;
				_increment[1] = 0.1;
				_increment[2] = 0.5;
				_increment[3] = 0.5;
				_increment[4] = 1;
				_increment[5] = 1;
				_increment[6] = 1;
				_increment[7] = 1;
				_increment[8] = 5;
				_increment[9] = 5;
				_increment[10] = 5;
				_increment[11] = 5;
				_increment[12] = 10;
				_increment[13] = 10;
				_increment[14] = 10;
				_increment[15] = 10;
			}

			public double this[int bitDepth]
			{
				get
				{
					if (bitDepth > 16)
						bitDepth = 16;
					if (bitDepth < 1)
						bitDepth = 1;
					return _increment[bitDepth - 1];
				}
			}
		}
	}
}