#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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
