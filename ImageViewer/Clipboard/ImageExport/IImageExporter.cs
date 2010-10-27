#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Desktop;

#pragma warning disable 0419,1574,1587,1591

namespace ClearCanvas.ImageViewer.Clipboard.ImageExport
{
	[ExtensionPoint]
	public sealed class ImageExporterExtensionPoint : ExtensionPoint<IImageExporter> {}

	public interface IImageExporter
	{
		string Identifier { get; }
		string Description { get; }
		string[] FileExtensions { get; }

		void Export(IPresentationImage image, string fileName, ExportImageParams exportParams);
	}

	public interface IConfigurableImageExporter : IImageExporter
	{
		IApplicationComponent GetConfigurationComponent();
	}
}