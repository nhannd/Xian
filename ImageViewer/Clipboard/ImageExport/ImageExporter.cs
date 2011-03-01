#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Drawing;
using System.Drawing.Imaging;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.Rendering;
using ClearCanvas.ImageViewer.StudyManagement;

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

			if (!(image is ISpatialTransformProvider) || !(image is IImageGraphicProvider))
				throw new ArgumentException("The image must implement IImageGraphicProvider and have a valid ImageSpatialTransform in order to be exported.");

			if (exportParams.ExportOption == ExportOption.TrueSize)
			{
				var imageSopProvider = image as IImageSopProvider;
				var pixelSpacing = imageSopProvider == null ? null : imageSopProvider.Frame.NormalizedPixelSpacing;
				if (pixelSpacing == null || pixelSpacing.IsNull)
					throw new ArgumentException("The image does not contain pixel spacing information.  TrueSize export is not possible.");
			}

			ImageSpatialTransform transform = ((ISpatialTransformProvider) image).SpatialTransform as ImageSpatialTransform;
			if (transform == null)
				throw new ArgumentException("The image must have a valid ImageSpatialTransform in order to be exported.");

			bool oldAnnotationLayoutVisible = false;
			IAnnotationLayoutProvider annotationLayout = image as IAnnotationLayoutProvider;
			if (annotationLayout != null && annotationLayout.AnnotationLayout != null)
			{
				oldAnnotationLayoutVisible = annotationLayout.AnnotationLayout.Visible;
				annotationLayout.AnnotationLayout.Visible = exportParams.ShowTextOverlay;
			}

			try
			{
				if (exportParams.ExportOption == ExportOption.TrueSize)
					return DrawTrueSizeImageToBitmap(image, exportParams.OutputSize, exportParams.Dpi);

				if (exportParams.SizeMode == SizeMode.Scale)
				{
					// TODO: Refactor ImageExporter, so there only the displayRectangle and OutputRectangle are provided
					//		Scale can be automatically figured out.
					//		A "Padded" option can be provided to distinguish between the current Fixed and ScaleToFit options
					// TODO: Refactor ImageExporter, so there are separate exporters for each ExportOption.
					//		The ExportImageParams is getting too many options and not all of them are applicable to each exporter
					//		Instead, each exporter should have its own parameters.

					if (exportParams.ExportOption == ExportOption.Wysiwyg)
					{
						return DrawWysiwygImageToBitmap(image, exportParams.DisplayRectangle, exportParams.Scale, exportParams.Dpi);
					}
					else
					{
						return DrawCompleteImageToBitmap(image, exportParams.Scale, exportParams.Dpi);
					}
				}
				else if (exportParams.SizeMode == SizeMode.ScaleToFit)
				{
					if (exportParams.ExportOption == ExportOption.Wysiwyg)
					{
						var scale = ScaleToFit(exportParams.DisplayRectangle.Size, exportParams.OutputSize);
						return DrawWysiwygImageToBitmap(image, exportParams.DisplayRectangle, scale, exportParams.Dpi);
					}
					else
					{
						var sourceImage = (IImageGraphicProvider)image;
						var scale = ScaleToFit(new Size(sourceImage.ImageGraphic.Columns, sourceImage.ImageGraphic.Rows), exportParams.OutputSize);
						return DrawCompleteImageToBitmap(image, scale, exportParams.Dpi);
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
							bmp = DrawWysiwygImageToBitmap(image, exportParams.DisplayRectangle, scale, exportParams.Dpi);
						}
						else
						{
							IImageGraphicProvider sourceImage = (IImageGraphicProvider) image;
							float scale = ScaleToFit(new Size(sourceImage.ImageGraphic.Columns, sourceImage.ImageGraphic.Rows), exportParams.OutputSize);
							bmp = DrawCompleteImageToBitmap(image, scale, exportParams.Dpi);
						}
						graphics.DrawImageUnscaledAndClipped(bmp, new Rectangle(CenterRectangles(bmp.Size, exportParams.OutputSize), bmp.Size));
						bmp.Dispose();
					}
					return paddedImage;
				}
			}
			finally
			{
				if (annotationLayout != null && annotationLayout.AnnotationLayout != null)
					annotationLayout.AnnotationLayout.Visible = oldAnnotationLayoutVisible;
			}
		}

		private static Bitmap DrawCompleteImageToBitmap(IPresentationImage image, float scale, float dpi)
		{
			ImageSpatialTransform transform = (ImageSpatialTransform)((ISpatialTransformProvider)image).SpatialTransform;
			object restoreMemento = transform.CreateMemento();
			try
			{
				ImageGraphic imageGraphic = ((IImageGraphicProvider) image).ImageGraphic;
				Rectangle imageRectangle = new Rectangle(0, 0, imageGraphic.Columns, imageGraphic.Rows);

				transform.Initialize();
				transform.ScaleToFit = false;
				transform.Scale = scale;
				RectangleF displayRectangle = imageGraphic.SpatialTransform.ConvertToDestination(imageRectangle);
				int width = (int) Math.Round(displayRectangle.Width);
				int height = (int) Math.Round(displayRectangle.Height);

				transform.ScaleToFit = true;
				return ImageDrawToBitmap(image, width, height, dpi);
			}
			finally
			{
				transform.SetMemento(restoreMemento);
			}
		}

		private static Bitmap DrawWysiwygImageToBitmap(IPresentationImage image, Rectangle displayRectangle, float scale, float dpi)
		{
			ImageSpatialTransform transform = (ImageSpatialTransform) ((ISpatialTransformProvider) image).SpatialTransform;
			object restoreMemento = transform.CreateMemento();
			try
			{
				int width = (int) (displayRectangle.Width*scale);
				int height = (int) (displayRectangle.Height*scale);

				transform.Scale *= scale;

				return ImageDrawToBitmap(image, width, height, dpi);
			}
			finally
			{
				transform.SetMemento(restoreMemento);
			}
		}

		private static Bitmap DrawTrueSizeImageToBitmap(IPresentationImage image, Size outputSize, float dpi)
		{
			var transform = (ImageSpatialTransform) ((ISpatialTransformProvider) image).SpatialTransform;
			var restoreMemento = transform.CreateMemento();
			try
			{
				var srcPixelSpacing = ((IImageSopProvider)image).Frame.NormalizedPixelSpacing;

				//TODO (CR March 2011) - Highest: Need to account for 90 degree rotation for images with non-square pixels.
				var srcDPI = 25.4f/srcPixelSpacing.Column;
				var scaleForTrueSize = dpi / srcDPI;

				// Apply scale transformation
				transform.ScaleToFit = false;
				transform.Scale = (float)scaleForTrueSize;

				return ImageDrawToBitmap(image, outputSize.Width, outputSize.Height, dpi);
			}
			finally
			{
				transform.SetMemento(restoreMemento);
			}
		}

		private static Bitmap ImageDrawToBitmap(IPresentationImage presentationImage, int width, int height, float dpi)
		{
			if (!(presentationImage is PresentationImage))
				return presentationImage.DrawToBitmap(width, height);

			var image = (PresentationImage) presentationImage;
			var bmp = new Bitmap(width, height);

			var graphics = System.Drawing.Graphics.FromImage(bmp);
			var contextId = graphics.GetHdc();
			try
			{
				using (var surface = image.ImageRenderer.GetRenderingSurface(IntPtr.Zero, bmp.Width, bmp.Height))
				{
					surface.ContextID = contextId;
					surface.ClipRectangle = new Rectangle(0, 0, bmp.Width, bmp.Height);

					var drawArgs = new DrawArgs(surface, null, DrawMode.Render) {Dpi = dpi};
					image.Draw(drawArgs);
					drawArgs = new DrawArgs(surface, null, DrawMode.Refresh) {Dpi = dpi};
					image.Draw(drawArgs);
				}
			}
			finally
			{
				graphics.ReleaseHdc(contextId);
				graphics.Dispose();
			}
			return bmp;
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