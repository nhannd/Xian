using ClearCanvas.Common;
using ClearCanvas.Desktop;
using System.Drawing;

namespace ClearCanvas.ImageViewer.Clipboard.ImageExport
{
	public enum ExportOption
	{
		Wysiwyg = 0,
		CompleteImage = 1
	}

	public class ExportImageParams
	{
		private readonly ExportOption _exportOption;
		private readonly Rectangle _displayRectangle;

		public ExportImageParams(ExportOption exportOption, Rectangle displayRectangle)
		{
			_exportOption = exportOption;
			_displayRectangle = displayRectangle;
		}

		public ExportOption ExportOption
		{
			get { return _exportOption; }
		}

		//for wysiwyg
		public Rectangle DisplayRectangle
		{
			get { return _displayRectangle; }
		}
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
