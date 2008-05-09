using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

#pragma warning disable 0419,1574,1587,1591

namespace ClearCanvas.ImageViewer.Clipboard.ImageExport
{
	public enum ExportOption
	{
		Wysiwyg = 0,
		CompleteImage = 1
	}

	public class ExportImageParams
	{
		public ExportImageParams()
		{
		}

		public ExportOption ExportOption = ImageExport.ExportOption.Wysiwyg;
		public Rectangle DisplayRectangle;
		public float Scale = 1F;
	}

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

	public sealed class ImageExporterExtensionPoint : ExtensionPoint<IImageExporter>
	{
		public ImageExporterExtensionPoint()
		{
		}
	}
}
