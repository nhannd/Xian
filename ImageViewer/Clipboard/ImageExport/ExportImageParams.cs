#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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
		/// This is ignored if <see cref="ExportOption"/> is <see cref="ImageExport.ExportOption.TrueSize"/>
		/// </summary>
		public float Scale = 1F;

		/// <summary>
		/// Specifies the output pixel spacing (in millimeter) when <see cref="ExportOption"/> has a value of <see cref="ImageExport.ExportOption.TrueSize"/>.
		/// </summary>
		public float OutputPixelSpacing;

		/// <summary>
		/// Specifies the output image dimensions when <see cref="SizeMode"/> has a value of <see cref="ImageExport.SizeMode.Fixed"/> or <see cref="ImageExport.SizeMode.ScaleToFit"/>.
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
		CompleteImage = 1,

		/// <summary>
		/// Indicates that the visible area of the image (including any rotation and/or flips) should be exported.
		/// The image is automatically scaled so the destination correspond to the physical size of the original image.
		/// </summary>
		TrueSize = 2
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
		/// The output image fit into the specified <see cref="ExportImageParams.OutputSize"/>.  There is no padding added.
		/// </summary>
		ScaleToFit,

		/// <summary>
		/// Indicates that the exported image should be scaled to fit a fixed size.
		/// The output image is padded to fill the specified <see cref="ExportImageParams.OutputSize"/>.
		/// </summary>
		Fixed
	}
}