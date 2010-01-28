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

namespace ClearCanvas.ImageViewer.Clipboard.ImageExport
{
	/// <summary>
	/// Specifies parameters for image export options.
	/// </summary>
	public class ExportImageParams
	{
		/// <summary>
		/// Specifies the subject area of the image to be exported.
		/// </summary>
		public ExportOption ExportOption = ExportOption.Wysiwyg;

		/// <summary>
		/// Specifies the visible area of the image.
		/// </summary>
		public Rectangle DisplayRectangle;

		/// <summary>
		/// Specifies the output sizing mode.
		/// </summary>
		public SizeMode SizeMode = SizeMode.Scale;

		/// <summary>
		/// Specifies the scaling factor when <see cref="SizeMode"/> has a value of <see cref="ImageExport.SizeMode.Scale"/>.
		/// </summary>
		public float Scale = 1F;

		/// <summary>
		/// Specifies the output image dimensions when <see cref="SizeMode"/> has a value of <see cref="ImageExport.SizeMode.Fixed"/>.
		/// </summary>
		public Size OutputSize;

		/// <summary>
		/// Specifies the colour with which to paint the background of the output image when <see cref="SizeMode"/> has a value of <see cref="ImageExport.SizeMode.Fixed"/>.
		/// </summary>
		public Color BackgroundColor;

		/// <summary>
		/// Specifies whether or not the text overlay annotation layer should be visible in the output.
		/// </summary>
		public bool ShowTextOverlay = false;
	}

	/// <summary>
	/// Enumerated values for specifying the subject area to be exported.
	/// </summary>
	public enum ExportOption
	{
		/// <summary>
		/// Indicates that only the visible area of the image (including any rotations and/or flips) should be exported.
		/// </summary>
		Wysiwyg = 0,

		/// <summary>
		/// Indicates that the entire image should be exported in the original image's orientation (i.e. excluding all rotations and/or flips).
		/// </summary>
		CompleteImage = 1
	}

	/// <summary>
	/// Enumerated values for specifying the image export sizing mode.
	/// </summary>
	public enum SizeMode
	{
		/// <summary>
		/// Indicates that the exported image should be scaled according to a specified factor.
		/// </summary>
		Scale,

		/// <summary>
		/// Indicates that the exported image should be scaled to fit a fixed size.
		/// </summary>
		Fixed
	}
}