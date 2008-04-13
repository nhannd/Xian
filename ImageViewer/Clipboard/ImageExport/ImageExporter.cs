using System;
using System.Drawing;
using System.Drawing.Imaging;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.Common.Utilities;

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
				
				if (exportParams.ExportOption == ExportOption.Wysiwyg)
				{
					int width = exportParams.DisplayRectangle.Width;
					int height = exportParams.DisplayRectangle.Height;
					return image.DrawToBitmap(width, height);
				}
				else
				{
					object restoreMemento = transform.CreateMemento();

					ImageGraphic imageGraphic = ((IImageGraphicProvider) image).ImageGraphic;
					Rectangle imageRectangle = new Rectangle(0, 0, imageGraphic.Columns, imageGraphic.Rows);

					transform.Initialize();
					transform.ScaleToFit = false;

					RectangleF displayRectangle = imageGraphic.SpatialTransform.ConvertToDestination(imageRectangle);
					int width = (int)Math.Round(displayRectangle.Width);
					int height = (int)Math.Round(displayRectangle.Height);

					transform.ScaleToFit = true;
					Bitmap bitmap = image.DrawToBitmap(width, height);
					
					//restore previous state.
					transform.SetMemento(restoreMemento);
					return bitmap;
				}
			}

			throw new ArgumentException("The image must implement IImageGraphicProvider and have a valid ImageSpatialTransform in order to be exported.");
		}

		public static void Export(IPresentationImage image, string filePath, ExportImageParams exportParams, ImageFormat imageFormat)
		{
			using (Bitmap bmp = DrawToBitmap(image, exportParams))
			{
				Export(bmp, filePath, imageFormat);
			}
		}

		public static void Export(IPresentationImage image, string filePath, ExportImageParams exportParams, ImageCodecInfo encoder, EncoderParameters encoderParameters)
		{
			using (Bitmap bmp = DrawToBitmap(image, exportParams))
			{
				Export(bmp, filePath, encoder, encoderParameters);
			}
		}

		public static void Export(Image image, string filePath, ImageFormat imageFormat)
		{
			Platform.CheckForNullReference(image, "image");
			Platform.CheckForNullReference(imageFormat, "imageFormat");
			Platform.CheckForEmptyString(filePath, "filePath");

			image.Save(filePath, imageFormat);
		}

		public static void Export(Image image, string filePath, ImageCodecInfo encoder, EncoderParameters encoderParameters)
		{
			Platform.CheckForNullReference(image, "image");
			Platform.CheckForEmptyString(filePath, "filePath");
			Platform.CheckForNullReference(encoder, "encoder");
			Platform.CheckForNullReference(encoderParameters, "encoderParameters");

			image.Save(filePath, encoder, encoderParameters);
		}
	}
}