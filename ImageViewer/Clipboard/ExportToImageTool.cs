using System.Collections.Generic;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.Clipboard.ImageExport;
using System;
using System.Threading;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Clipboard
{
	[MenuAction("export", "clipboard-contextmenu/MenuExportToImage", "Export")]
	[ButtonAction("export", "clipboard-toolbar/ToolbarExportToImage", "Export")]
	[Tooltip("export", "TooltipExportToImage")]
	[IconSet("export", IconScheme.Colour, "Icons.ExportToImageToolSmall.png", "Icons.ExportToImageToolSmall.png", "Icons.ExportToImageToolSmall.png")]
	[EnabledStateObserver("delete", "Enabled", "EnabledChanged")]

	[ExtensionOf(typeof(ClipboardToolExtensionPoint))]
	public class ExportToImageTool : ClipboardTool
	{
		#region Progress Shelf and Component fields

		private static IShelf _progressComponentShelf;
		private static ProgressDialogComponent _progressComponent;

		#endregion

		#region Export Data fields

		private static volatile int _numberOfImagesToExport;
		private static volatile List<IClipboardItem> _exportItems;
		private static volatile IImageExporter _exporter;
		private static volatile string _outputDirectory;
		private static volatile ExportOption _exportOption;
		private static Exception _error;

		#endregion
		
		public ExportToImageTool()
		{
		}

		public override void Initialize()
		{
			this.Enabled = this.Context.SelectedClipboardItems.Count > 0;
			base.Initialize();
		}

		public void Export()
		{
			try
			{
				ExportInternal();
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Context.DesktopWindow);
			}
		}

		#region Export Methods

		private void ExportInternal()
		{
			if (_progressComponent != null)
			{
				this.Context.DesktopWindow.ShowMessageBox(SR.MessageExportStillRunning, MessageBoxActions.Ok);
				return;
			}

			int numberOfImagesToExport = GetNumberOfImagesToExport();
			if (numberOfImagesToExport == 0)
				return;

			string title = SR.TitleExportImages;
			if (numberOfImagesToExport == 1)
				title = SR.TitleExportSingleImage;

			ImageExportComponent component = new ImageExportComponent(numberOfImagesToExport);
			if (ApplicationComponentExitCode.Accepted !=
				ApplicationComponent.LaunchAsDialog(this.Context.DesktopWindow, component, title))
			{
				return;
			}

			ExportInternal(component);
		}

		private void ExportInternal(ImageExportComponent component)
		{
			IImageExporter exporter = component.SelectedExporter;
			if (exporter == null)
				throw new InvalidOperationException("No exporter was chosen; unable to export any images.");

			if (component.NumberOfImagesToExport == 1)
			{
				if (!Directory.Exists(System.IO.Path.GetDirectoryName(component.ExportFilePath ?? "")))
					throw new FileNotFoundException("The specified export directory does not exist.");

				ClipboardItem clipboardItem = (ClipboardItem)this.Context.SelectedClipboardItems[0];

				exporter.Export((IPresentationImage)clipboardItem.Item, component.ExportFilePath,
									new ExportImageParams(component.ExportOption, clipboardItem.DisplayRectangle));
			}
			else
			{
				if (!Directory.Exists(component.ExportFilePath ?? ""))
					throw new FileNotFoundException("The specified export directory does not exist.");

				InitializeMultipleExportItems(component);

				if (component.NumberOfImagesToExport <= 5)
				{
					BlockingOperation.Run(delegate { ExportMultipleImages(null); });
					Cleanup();
				}
				else
				{
					BackgroundTask task = new BackgroundTask(ExportMultipleImages, true);

					_progressComponent = new ProgressDialogComponent(task, true, ProgressBarStyle.Blocks);
					_progressComponentShelf = ApplicationComponent.LaunchAsShelf(
						this.Context.DesktopWindow,
						_progressComponent, 
						SR.TitleExportingImages, "ExportImages", 
						ShelfDisplayHint.DockFloat);

					_progressComponentShelf.Closed +=
						delegate
							{
									Cleanup();
									task.Dispose();
							};
				}
			}
		}

		#region Shared Sync/Async Export method

		private static void ExportMultipleImages(IBackgroundTaskContext context)
		{
			int progress = 0;

			try
			{
				MultipleImageExportFileNamingStrategy fileNamingStrategy = new MultipleImageExportFileNamingStrategy(_outputDirectory);

				string fileExtension = _exporter.FileExtensions[0];
				ReportProgress(context, SR.MessageExportingImages, progress);

				foreach (ClipboardItem clipboardItem in _exportItems)
				{
					if (context != null && context.CancelRequested)
						break;

					ExportImageParams exportParams = new ExportImageParams(_exportOption, clipboardItem.DisplayRectangle);

					if (clipboardItem.Item is IPresentationImage)
					{
						IPresentationImage image = (IPresentationImage) clipboardItem.Item;
						string fileName = fileNamingStrategy.GetSingleImageFileName(image, fileExtension);
						_exporter.Export(image, fileName, exportParams);

						ReportProgress(context, fileName, ++progress);
					}
					else if (clipboardItem.Item is IDisplaySet)
					{
						IDisplaySet displaySet = (IDisplaySet) clipboardItem.Item;
						foreach (ImageFileNamePair pair in fileNamingStrategy.GetImagesAndFileNames(displaySet, fileExtension))
						{
							if (context != null && context.CancelRequested)
								break;

							_exporter.Export(pair.Image, pair.FileName, exportParams);

							ReportProgress(context, pair.FileName, ++progress);
						}
					}
				}

				if (context != null && context.CancelRequested)
					ReportProgress(context, SR.MessageCancelled, progress);
				else
					ReportProgress(context, SR.MessageExportComplete, progress);
			}
			catch(Exception e)
			{
				_error = e;
				Platform.Log(LogLevel.Error, e);
			}

			if (context != null)
				context.Complete();
		}

		private static void ReportProgress(IBackgroundTaskContext context, string message, int currentStep)
		{
			if (context == null)
				return;

			int percent = Math.Min((int)(currentStep/(float) _numberOfImagesToExport * 100), 100);
			context.ReportProgress(new BackgroundTaskProgress(percent, message));
		}

		#endregion

		#endregion

		private void InitializeMultipleExportItems(ImageExportComponent component)
		{
			_numberOfImagesToExport = component.NumberOfImagesToExport;
			_exportItems = new List<IClipboardItem>(this.Context.SelectedClipboardItems);
			_exporter = component.SelectedExporter;
			_outputDirectory = component.ExportFilePath;
			_exportOption = component.ExportOption;
		}

		private void Cleanup()
		{
			_progressComponentShelf = null;
			_progressComponent = null;

			_numberOfImagesToExport = 0;
			_exportItems = null;
			_exporter = null;
			_outputDirectory = null;

			if (_error != null)
			{
				this.Context.DesktopWindow.ShowMessageBox(SR.MessageExportFailed, MessageBoxActions.Ok);
				_error = null;
			}
		}

		private int GetNumberOfImagesToExport()
		{
			int number = 0;
			foreach (ClipboardItem clipboardItem in this.Context.SelectedClipboardItems)
			{
				if (clipboardItem.Item is IPresentationImage)
				{
					++number;
				}
				else if (clipboardItem.Item is IDisplaySet)
				{
					number += ((IDisplaySet)clipboardItem.Item).PresentationImages.Count;
				}
			}

			return number;
		}
	}
}
