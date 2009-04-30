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

using System.Collections.Generic;
using System.Drawing.Imaging;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Clipboard.ImageExport
{
	internal static class StandardImageExporterFactory
	{
		public static List<IImageExporter> CreateStandardExporters()
		{
			List<IImageExporter> exporters = new List<IImageExporter>();
			
			exporters.Add(new StandardImageExporter("Jpg", "DescriptionJpg", ImageFormat.Jpeg, new string[] { "jpg", "jpeg" }));
			exporters.Add(new StandardImageExporter("Png", "DescriptionPng", ImageFormat.Png, new string[] { "png" }));
			exporters.Add(new StandardImageExporter("Bmp", "DescriptionBmp", ImageFormat.Bmp, new string[] { "bmp" }));
			exporters.Add(new StandardImageExporter("Gif", "DescriptionGif", ImageFormat.Gif, new string[] { "gif" }));
			exporters.Add(new StandardImageExporter("Tiff", "DescriptionTiff", ImageFormat.Tiff, new string[] { "tif", "tiff" }));

			return exporters;
		}
	}

	internal class StandardImageExporter : ImageExporter
	{
		private readonly ImageFormat _imageFormat;

		internal StandardImageExporter( string identifier, string description, ImageFormat imageFormat, string[] fileExtensions)
			: base(identifier, description, fileExtensions)
		{
			Platform.CheckForNullReference(imageFormat, "imageFormat");
			_imageFormat = imageFormat;
		}

		public override void Export(IPresentationImage image, string filePath, ExportImageParams exportParams)
		{
			Export(image, filePath, exportParams, _imageFormat);
		}
	}
}
