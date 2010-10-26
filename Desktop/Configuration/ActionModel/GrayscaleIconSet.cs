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
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Configuration.ActionModel
{
	//TODO: might be useful in core, but not right now.
	internal class GrayscaleIconSet : IconSet
	{
		private readonly IconSet _source;

		public GrayscaleIconSet(IconSet source)
			: base(IconScheme.Monochrome, source.SmallIcon, source.MediumIcon, source.LargeIcon)
		{
			_source = source;
		}

		public override Image CreateIcon(IconSize iconSize, IResourceResolver resourceResolver)
		{
			//Already gray.
			if (_source.Scheme == IconScheme.Monochrome)
				return _source.CreateIcon(iconSize, resourceResolver);
			
			//TODO: make unsafe. Not enabling unsafe code just for this, though.
			var colorImage = (Bitmap)base.CreateIcon(iconSize, resourceResolver);
			for (int x = 0; x < colorImage.Width; ++x)
			{
				for (int y = 0; y < colorImage.Height; ++y)
				{
					var pixel = colorImage.GetPixel(x, y);
					int gray = (int)(pixel.R * 0.3f + pixel.G * 0.59F + pixel.B * 0.11f);
					if (gray > 255)
						gray = 255;

					colorImage.SetPixel(x, y, Color.FromArgb(pixel.A, gray, gray, gray));
				}
			}

			return colorImage;
		}

		public override string GetIconKey(IconSize iconSize, IResourceResolver resourceResolver)
		{
			if (_source.Scheme == IconScheme.Monochrome)
				return base.GetIconKey(iconSize, resourceResolver);

			return base.GetIconKey(iconSize, resourceResolver) + "_ConvertedToGrayscale";
		}
	}
}