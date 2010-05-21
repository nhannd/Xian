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
using System.Collections.Generic;
using System.Drawing; 

namespace ClearCanvas.ImageViewer.AdvancedImaging.Fusion
{
	public enum StandardColorMaps
	{
		HotMetal,
		FluidJet,
		Spectrum
	}

	partial class InterpolatedColorMap
	{
		public InterpolatedColorMap(StandardColorMaps colorMaps)
		{
			List<KeyValuePair<float, Color>> list = new List<KeyValuePair<float, Color>>();
			switch (colorMaps)
			{
				case StandardColorMaps.HotMetal:
					list.Add(new KeyValuePair<float, Color>(0/6f, Color.FromArgb(0x52, 0x00, 0x00)));
					list.Add(new KeyValuePair<float, Color>(2/6f, Color.FromArgb(0xFF, 0x00, 0x00)));
					list.Add(new KeyValuePair<float, Color>(4/6f, Color.FromArgb(0xFF, 0xA5, 0x00)));
					list.Add(new KeyValuePair<float, Color>(5/6f, Color.FromArgb(0xFF, 0xFF, 0x00)));
					list.Add(new KeyValuePair<float, Color>(6/6f, Color.FromArgb(0xFF, 0xFF, 0xBD)));
					break;
				case StandardColorMaps.FluidJet:
					list.Add(new KeyValuePair<float, Color>(00/15f, Color.FromArgb(0x00, 0x00, 0xBD)));
					list.Add(new KeyValuePair<float, Color>(01/15f, Color.FromArgb(0x00, 0x00, 0xFF)));
					list.Add(new KeyValuePair<float, Color>(02/15f, Color.FromArgb(0x00, 0x43, 0xFF)));
					list.Add(new KeyValuePair<float, Color>(03/15f, Color.FromArgb(0x00, 0x84, 0xFF)));
					list.Add(new KeyValuePair<float, Color>(04/15f, Color.FromArgb(0x00, 0xC1, 0xFF)));
					list.Add(new KeyValuePair<float, Color>(05/15f, Color.FromArgb(0x00, 0xFF, 0xFF)));
					list.Add(new KeyValuePair<float, Color>(06/15f, Color.FromArgb(0x44, 0xFF, 0xBB)));
					list.Add(new KeyValuePair<float, Color>(07/15f, Color.FromArgb(0x84, 0xFF, 0x84)));
					list.Add(new KeyValuePair<float, Color>(08/15f, Color.FromArgb(0xBD, 0xFF, 0x42)));
					list.Add(new KeyValuePair<float, Color>(09/15f, Color.FromArgb(0xFF, 0xFF, 0x00)));
					list.Add(new KeyValuePair<float, Color>(10/15f, Color.FromArgb(0xFF, 0xBD, 0x00)));
					list.Add(new KeyValuePair<float, Color>(11/15f, Color.FromArgb(0xFF, 0x84, 0x00)));
					list.Add(new KeyValuePair<float, Color>(12/15f, Color.FromArgb(0xFF, 0x40, 0x00)));
					list.Add(new KeyValuePair<float, Color>(13/15f, Color.FromArgb(0xFF, 0x00, 0x00)));
					list.Add(new KeyValuePair<float, Color>(14/15f, Color.FromArgb(0xBD, 0x00, 0x00)));
					list.Add(new KeyValuePair<float, Color>(15/15f, Color.FromArgb(0x84, 0x00, 0x00)));
					break;
				case StandardColorMaps.Spectrum:
					list.Add(new KeyValuePair<float, Color>(00/13f, Color.FromArgb(0xFF, 0x00, 0x00)));
					list.Add(new KeyValuePair<float, Color>(01/13f, Color.FromArgb(0xFF, 0x63, 0x00)));
					list.Add(new KeyValuePair<float, Color>(02/13f, Color.FromArgb(0xFF, 0xC0, 0x00)));
					list.Add(new KeyValuePair<float, Color>(03/13f, Color.FromArgb(0xDE, 0xFF, 0x00)));
					list.Add(new KeyValuePair<float, Color>(04/13f, Color.FromArgb(0x7E, 0xFF, 0x00)));
					list.Add(new KeyValuePair<float, Color>(05/13f, Color.FromArgb(0x21, 0xFF, 0x00)));
					list.Add(new KeyValuePair<float, Color>(06/13f, Color.FromArgb(0x00, 0xFF, 0x42)));
					list.Add(new KeyValuePair<float, Color>(07/13f, Color.FromArgb(0x00, 0xFF, 0xA0)));
					list.Add(new KeyValuePair<float, Color>(08/13f, Color.FromArgb(0x00, 0xFF, 0xFF)));
					list.Add(new KeyValuePair<float, Color>(09/13f, Color.FromArgb(0x00, 0x9C, 0xFF)));
					list.Add(new KeyValuePair<float, Color>(10/13f, Color.FromArgb(0x00, 0x42, 0xFF)));
					list.Add(new KeyValuePair<float, Color>(11/13f, Color.FromArgb(0x21, 0x00, 0xFF)));
					list.Add(new KeyValuePair<float, Color>(12/13f, Color.FromArgb(0x7F, 0x00, 0xFF)));
					list.Add(new KeyValuePair<float, Color>(13/13f, Color.FromArgb(0xDE, 0x00, 0xFF)));
					break;
				default:
					throw new NotSupportedException();
			}
			_fixedNodes = list.AsReadOnly();
			_name = colorMaps.ToString();
		}
	}
}