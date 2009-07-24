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
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Presentation
{
	/// <summary>
	/// Describes whatever Lut is currently applied to a presentation image.
	/// </summary>
	/// <remarks>
	/// At first glance, you might think this belongs in the Dicom namespace within this project.
	/// However, the information in the Dicom header only applies to the initial presentation of
	/// the image and is not representative of the current state of the image in the viewport.
	/// The user could have changed the W/L, applied a custom Data Lut, or a Lut from a related
	/// Grayscale Presentation State object could be applied.
	/// </remarks>
	internal sealed class AppliedLutAnnotationItem : AnnotationItem
	{
		public AppliedLutAnnotationItem()
			: base("Presentation.AppliedLut", new AnnotationResourceResolver(typeof(AppliedLutAnnotationItem).Assembly))
		{
		}

		public override string GetAnnotationText(IPresentationImage presentationImage)
		{
			if (presentationImage == null)
				return String.Empty;

			IVoiLutProvider image = presentationImage as IVoiLutProvider;

			if (image == null || !image.VoiLutManager.Enabled)
				return String.Empty;

			IComposableLut voiLut = image.VoiLutManager.GetLut();
			if (voiLut == null)
				return String.Empty;

			return voiLut.GetDescription();
		}
	}
}
