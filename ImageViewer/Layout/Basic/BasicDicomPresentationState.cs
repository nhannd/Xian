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
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.PresentationStates;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
	/// <summary>
	/// A <see cref="DicomPresentationState"/> class that applies very simple transformations to
	/// the default presentation state of a DICOM-based <see cref="PresentationImage"/>.
	/// </summary>
	/// <remarks>Eventually, this will be expanded to support all of the Presentation
	/// Intent attributes from DICOM Supplement 60, but right now it only contains the
	/// "Show Grayscale Inverted" attribute.</remarks>
	[Cloneable(true)]
	internal class BasicDicomPresentationState : PresentationState
	{
		/// <summary>
		/// Initializes an instance of <see cref="BasicDicomPresentationState"/>.
		/// </summary>
		public BasicDicomPresentationState()
		{}

		public bool ShowGrayscaleInverted { get; set; }

		/// <summary>
		/// Not supported.
		/// </summary>
		/// <exception cref="NotSupportedException">Thrown always.</exception>
		public override void Serialize(IEnumerable<IPresentationImage> images)
		{
			throw new System.NotSupportedException("Simple presentation state objects cannot be serialized.");
		}

		/// <summary>
		/// Deserializes the presentation state, applying the changes to each image
		/// in <paramref name="images"/>.
		/// </summary>
		/// <remarks>
		/// The presentation state applied by <see cref="DicomSoftcopyPresentationState.Default"/> is
		/// deserialized first, followed by any specializations of this instance.
		/// </remarks>
		public override void Deserialize(IEnumerable<IPresentationImage> images)
		{
			DicomDefault.Deserialize(images);

			if (ShowGrayscaleInverted)
			{
				foreach (IPresentationImage image in images)
				{
					if (image is IImageGraphicProvider)
					{
						if (!(((IImageGraphicProvider)image).ImageGraphic.PixelData is GrayscalePixelData))
							continue;
					}

					if (!(image is IVoiLutProvider))
						continue;

					IVoiLutManager manager = ((IVoiLutProvider)image).VoiLutManager;
					//Invert can be true by default depending on photometric interpretation,
					//so we'll just assume the current value is the default and flip it.
					manager.Invert = !manager.Invert;
				}
			}
		}

		public override void Clear(IEnumerable<IPresentationImage> image)
		{
		}
	}
}