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
using System.Drawing;
using System.Drawing.Imaging;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Mathematics;

#pragma warning disable 0419,1574,1587,1591

namespace ClearCanvas.ImageViewer.Clipboard.ImageExport
{
	public abstract class ImageExporter : IImageExporter
	{
		private readonly string _identifier;
		private readonly string _description;
		private readonly string[] _fileExtensions;

		protected ImageExporter(string identifier, string description, string[] fileExtensions)
		{
			Platform.CheckForEmptyString(identifier, "identifier");
			Platform.CheckForEmptyString(description, "description");
			Platform.CheckForNullReference(fileExtensions, "fileExtension");
			if (fileExtensions.Length == 0)
				throw new ArgumentException("The exporter must have at least one associated file extension.");

			IResourceResolver resolver = new ResourceResolver(this.GetType().Assembly);
			_identifier = identifier;
			_description = resolver.LocalizeString(description);
			_fileExtensions = fileExtensions;
		}

		#region IImageExporter Members

		public string Identifier
		{
			get { return _identifier; }
		}

		public string Description
		{
			get { return _description; }
		}

		public string[] FileExtensions
		{
			get { return _fileExtensions; }
		}

		public abstract void Export(IPresentationImage image, string filePath, ExportImageParams exportParams);

		#endregion

		public static Bitmap DrawToBitmap(IPresentationImage image, ExportImageParams exportParams)
		{
			Platform.CheckForNullReference(image, "image");
			Platform.CheckForNullReference(exportParams, "exportParams");
			
			if (image is ISpatialTransformProvider && image is IImageGraphicProvider)
			{
				ImageSpatialTransform transform = ((ISpatialTransformProvider)image).SpatialTransform as ImageSpatialTransform;
				if (transform == null)
					throw new ArgumentException("The image must have a valid ImageSpatialTransform in order to be exported.");

				if (exportParams.SizeMode == SizeMode.Scale)
				{
					if (exportParams.ExportOption == ExportOption.Wysiwyg)
					{
						return DrawWysiwygImageToBitmap(image, exportParams.DisplayRectangle, exportParams.Scale);
					}
					else
					{
						return DrawCompleteImageToBitmap(image, exportParams.Scale);
					}
				}
				else
				{
					Bitmap paddedImage = new Bitmap(exportParams.OutputSize.Width, exportParams.OutputSize.Height);
					using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(paddedImage))
					{
						// paint background
						using (Brush b = new SolidBrush(exportParams.BackgroundColor))
						{
							graphics.FillRectangle(b, new Rectangle(Point.Empty, exportParams.OutputSize));
						}

						// paint image portion
						Bitmap bmp;
						if (exportParams.ExportOption == ExportOption.Wysiwyg)
						{
							float scale = ScaleToFit(exportParams.DisplayRectangle.Size, exportParams.OutputSize);
							bmp = DrawWysiwygImageToBitmap(image, exportParams.DisplayRectangle, scale);
						}
						else
						{
							IImageGraphicProvider sourceImage = (IImageGraphicProvider) image;
							float scale = ScaleToFit(new Size(sourceImage.ImageGraphic.Columns, sourceImage.ImageGraphic.Rows), exportParams.OutputSize);
							bmp = DrawCompleteImageToBitmap(image, scale);
						}
						graphics.DrawImageUnscaledAndClipped(bmp, new Rectangle(CenterRectangles(bmp.Size, exportParams.OutputSize), bmp.Size));
						bmp.Dispose();
					}
					return paddedImage;
				}
			}

			throw new ArgumentException("The image must implement IImageGraphicProvider and have a valid ImageSpatialTransform in order to be exported.");
		}

		private static Bitmap DrawCompleteImageToBitmap(IPresentationImage image, float scale)
		{
			ImageSpatialTransform transform = (ImageSpatialTransform)((ISpatialTransformProvider)image).SpatialTransform;
			object restoreMemento = transform.CreateMemento();

			ImageGraphic imageGraphic = ((IImageGraphicProvider)image).ImageGraphic;
			Rectangle imageRectangle = new Rectangle(0, 0, imageGraphic.Columns, imageGraphic.Rows);

			transform.Initialize();
			transform.ScaleToFit = false;
			transform.Scale = scale;
			RectangleF displayRectangle = imageGraphic.SpatialTransform.ConvertToDestination(imageRectangle);
			int width = (int)Math.Round(displayRectangle.Width);
			int height = (int)Math.Round(displayRectangle.Height);

			transform.ScaleToFit = true;
			try
			{
				return image.DrawToBitmap(width, height);
			}
			finally
			{
				transform.SetMemento(restoreMemento);
			}
		}

		private static Bitmap DrawWysiwygImageToBitmap(IPresentationImage image, Rectangle displayRectangle, float scale)
		{
			ImageSpatialTransform transform = (ImageSpatialTransform)((ISpatialTransformProvider)image).SpatialTransform;
			object restoreMemento = transform.CreateMemento();

			int width = (int)(displayRectangle.Width * scale);
			int height = (int)(displayRectangle.Height * scale);

			transform.Scale *= scale;

			try
			{
				return image.DrawToBitmap(width, height);
			}
			finally
			{
				transform.SetMemento(restoreMemento);
			}
		}

		private static float ScaleToFit(Size source, SizeF destination)
		{
			if (source.Width == 0 || source.Height == 0)
				return 1;

			float aW = destination.Width/source.Width;
			float aH = destination.Height/source.Height;
			if (!FloatComparer.IsGreaterThan(aW * source.Height, destination.Height))
				return aW;
			else
				return aH;
		}

		private static Point CenterRectangles(Size source, Size destination)
		{
			return new Point((destination.Width - source.Width)/2, (destination.Height - source.Height)/2);
		}

		protected static void Export(IPresentationImage image, string filePath, ExportImageParams exportParams, ImageFormat imageFormat)
		{
			using (Bitmap bmp = DrawToBitmap(image, exportParams))
			{
				Export(bmp, filePath, imageFormat);
			}
		}

		protected static void Export(IPresentationImage image, string filePath, ExportImageParams exportParams, ImageCodecInfo encoder, EncoderParameters encoderParameters)
		{
			using (Bitmap bmp = DrawToBitmap(image, exportParams))
			{
				Export(bmp, filePath, encoder, encoderParameters);
			}
		}

		protected static void Export(Image image, string filePath, ImageFormat imageFormat)
		{
			Platform.CheckForNullReference(image, "image");
			Platform.CheckForNullReference(imageFormat, "imageFormat");
			Platform.CheckForEmptyString(filePath, "filePath");

			image.Save(filePath, imageFormat);
		}

		protected static void Export(Image image, string filePath, ImageCodecInfo encoder, EncoderParameters encoderParameters)
		{
			Platform.CheckForNullReference(image, "image");
			Platform.CheckForEmptyString(filePath, "filePath");
			Platform.CheckForNullReference(encoder, "encoder");
			Platform.CheckForNullReference(encoderParameters, "encoderParameters");

			image.Save(filePath, encoder, encoderParameters);
		}
	}
}