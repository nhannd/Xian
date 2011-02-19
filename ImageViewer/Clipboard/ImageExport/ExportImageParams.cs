#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Clipboard.ImageExport
{
	/// <summary>
	/// Specifies parameters for image export options.
	/// </summary>
	public class ExportImageParams
	{
		private ExportOption _exportOption = ExportOption.Wysiwyg;
		private SizeMode _sizeMode = SizeMode.Scale;
		private Rectangle _displayRectangle;
		private Size _outputSize;
		private float _scale = 1;
		private float _dpi = 96;

		/// <summary>
		/// Specifies the subject area of the image to be exported.
		/// </summary>
		public ExportOption ExportOption
		{
			get { return _exportOption; }
			set { _exportOption = value; }
		}

		/// <summary>
		/// Specifies the output resolution in DPI.
		/// </summary>
		public float Dpi
		{
			get { return _dpi; }
			set
			{
				Platform.CheckPositive(_dpi, "Dpi");
				_dpi = value;
			}
		}

		/// <summary>
		/// Specifies the visible area of the image.
		/// </summary>
		public Rectangle DisplayRectangle
		{
			get { return _displayRectangle; }
			set
			{
				if (!value.IsEmpty)
				{
					Platform.CheckPositive(value.Width, "Width");
					Platform.CheckPositive(value.Height, "Height");
				}
				_displayRectangle = value;
			}
		}

		/// <summary>
		/// Specifies the output sizing mode.
		/// </summary>
		public SizeMode SizeMode
		{
			get { return _sizeMode; }
			set { _sizeMode = value; }
		}

		/// <summary>
		/// Specifies the scaling factor when <see cref="SizeMode"/> has a value of <see cref="ImageExport.SizeMode.Scale"/>.
		/// </summary>
		public float Scale
		{
			get { return _scale; }
			set
			{
				Platform.CheckTrue(!FloatComparer.AreEqual(0, value), "Scale cannot be 0.");
				_scale = value;
			}
		}

		/// <summary>
		/// Specifies the output image dimensions when <see cref="SizeMode"/> has a value of <see cref="ImageExport.SizeMode.ScaleToFit"/> and <see cref="ImageExport.SizeMode.Fixed"/>.
		/// This is also used when the <see cref="ExportOption"/> is <see cref="ImageExport.ExportOption.TrueSize"/>.
		/// </summary>
		public Size OutputSize
		{
			get { return _outputSize; }
			set
			{
				if (!value.IsEmpty)
				{
					Platform.CheckPositive(value.Width, "Width");
					Platform.CheckPositive(value.Height, "Height");
				}
				_outputSize = value;
			}
		}

		/// <summary>
		/// Specifies the colour with which to paint the background of the output image when <see cref="SizeMode"/> has a value of <see cref="ImageExport.SizeMode.Fixed"/>.
		/// </summary>
		public Color BackgroundColor { get; set; }

		/// <summary>
		/// Specifies whether or not the text overlay annotation layer should be visible in the output.
		/// </summary>
		public bool ShowTextOverlay { get; set; }
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
		/// Indicates that the image will be scaled based on the <see cref="ExportImageParams.Dpi"/>.
		/// The center of the visible area of the image (including any rotations and/or flips) should remain visible..
		/// </summary>
		/// <remarks>
		/// <see cref="SizeMode"/> is irrelevant for TrueSize printing.
		/// </remarks>
		TrueSize = 2,
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
		/// The output image fit into a specified size.  There is no padding added.
		/// </summary>
		ScaleToFit,

		/// <summary>
		/// Indicates that the exported image should be scaled to fit a fixed size.
		/// The output image is padded to fill a specified size.
		/// </summary>
		Fixed
	}
}