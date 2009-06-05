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
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.RoiGraphics.Analyzers
{
	[ExtensionOf(typeof (RoiAnalyzerExtensionPoint))]
	public class RoiAreaAnalyzer : IRoiAnalyzer
	{
		private Units _units = Units.Centimeters;

		public Units Units
		{
			get { return _units; }
			set { _units = value; }
		}

		public bool SupportsRoi(Roi roi)
		{
			return roi is IRoiAreaProvider;
		}

		public string Analyze(Roi roi, RoiAnalysisMode mode)
		{
			if (!SupportsRoi(roi))
				return null;

			// performance enhancement to restrict excessive computation of polygon area.
			if (mode == RoiAnalysisMode.Responsive)
			{
				if (_units == Units.Pixels)
					return String.Format(SR.FormatAreaPixels, SR.StringNoValue);
				else if (_units == Units.Millimeters)
					return String.Format(SR.FormatAreaSquareMm, SR.StringNoValue);
				else
					return String.Format(SR.FormatAreaSquareCm, SR.StringNoValue);
			}

			IRoiAreaProvider areaProvider = (IRoiAreaProvider) roi;

			string text;

			Units oldUnits = areaProvider.Units;
			areaProvider.Units = areaProvider.IsCalibrated ? _units : Units.Pixels;

			if (!areaProvider.IsCalibrated || _units == Units.Pixels)
				text = String.Format(SR.FormatAreaPixels, areaProvider.Area);
			else if (_units == Units.Millimeters)
				text = String.Format(SR.FormatAreaSquareMm, areaProvider.Area);
			else
				text = String.Format(SR.FormatAreaSquareCm, areaProvider.Area);

			areaProvider.Units = oldUnits;

			return text;
		}
	}
}