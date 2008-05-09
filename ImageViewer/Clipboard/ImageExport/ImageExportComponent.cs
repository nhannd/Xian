#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using System.Collections.Generic;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Validation;
using Path=System.IO.Path;

#pragma warning disable 0419,1574,1587,1591

namespace ClearCanvas.ImageViewer.Clipboard.ImageExport
{
	[ExtensionPoint]
	public sealed class ImageExportComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	#region ImageExporterInfo class

	public class ImageExporterInfo
	{
		private readonly IImageExporter _imageExporter;

		internal ImageExporterInfo(IImageExporter imageExporter)
		{
			_imageExporter = imageExporter;
		}

		internal IImageExporter ImageExporter
		{
			get { return _imageExporter; }
		}

		public string Description
		{
			get { return _imageExporter.Description; }
		}

		public string FileExtensionFilter
		{
			get
			{
				string filterPortion = StringUtilities.Combine(_imageExporter.FileExtensions, ";",
													delegate(string extension)
													{
														return String.Format("*.{0}", extension);
													});

				return String.Format("{0}|{1}", _imageExporter.Description, filterPortion);
			}
		}

		public string DefaultExtension
		{
			get { return _imageExporter.FileExtensions[0]; }
		}

		public bool IsConfigurable
		{
			get { return _imageExporter is IConfigurableImageExporter; }
		}
	}

	#endregion

	[AssociateView(typeof(ImageExportComponentViewExtensionPoint))]
	public class ImageExportComponent : ApplicationComponent
	{
		private static IShelf _progressComponentShelf;

		private List<ImageExporterInfo> _imageExporters;

		private volatile ImageExporterInfo _selectedImageExporter;
		private readonly List<IClipboardItem> _itemsToExport;
		private readonly int _numberOfImagesToExport;
		private volatile string _exportFilePath;
		private volatile ExportOption _exportOption;
		private volatile float _scale = 1;
		private volatile Exception _error;

		private ImageExportComponent(List<IClipboardItem> itemsToExport, int numberOfImagesToExport)
		{
			_itemsToExport = itemsToExport;
			_numberOfImagesToExport = numberOfImagesToExport;
		}

		#region Component

		private IImageExporter SelectedExporter
		{
			get
			{
				if (_selectedImageExporter == null)
					return null;

				return _selectedImageExporter.ImageExporter;
			}
		}

		[ValidationMethodFor("ExportFilePath")]
		private ValidationResult ValidatePath()
		{
			bool valid = true;
			string message = null;

			if (NumberOfImagesToExport == 1)
			{
				string correctedFilename = GetCorrectedExportFilePath();
				string directory = Path.GetDirectoryName(ExportFilePath);

				if (!String.IsNullOrEmpty(directory) && Directory.Exists(directory))
				{
					string fileName = Path.GetFileName(correctedFilename);
					if (String.IsNullOrEmpty(fileName))
					{
						valid = false;
						message = SR.MessageInvalidFilePath;
					}
				}
				else
				{
					valid = false;
					message = SR.MessageInvalidFilePath;
				}
			}
			else
			{
				valid = (!String.IsNullOrEmpty(ExportFilePath) && Directory.Exists(ExportFilePath));

				if (!valid)
				{
					message = SR.MessageDirectoryDoesNotExist;
				}
			}

			if (valid)
				return new ValidationResult(true, "");
			else
				return new ValidationResult(false, message);
		}

		public override void Start()
		{
			LoadExporters();
			SetDefaults();

			base.Start();
		}
		
		#region Presentation Model

		public ICollection<ImageExporterInfo> ImageExporters
		{
			get { return _imageExporters; }
		}

		public ImageExporterInfo SelectedImageExporter
		{
			get { return _selectedImageExporter; }
			set
			{
				if (!_imageExporters.Contains(value))
					throw new ArgumentException("The specified image exporter does not exist.");

				_selectedImageExporter = value;

				NotifyPropertyChanged("SelectedImageExporter");
				NotifyPropertyChanged("ConfigureEnabled");
			}
		}

		public int NumberOfImagesToExport
		{
			get { return _numberOfImagesToExport; }
		}

		public bool ExportFilePathEnabled
		{
			get { return false; }	
		}

		public string ExportFilePathLabel
		{
			get { return _numberOfImagesToExport > 1 ? SR.LabelExportPath : SR.LabelExportFile; }
		}

		public string ExportFilePath
		{
			get { return _exportFilePath; }
			set
			{
				_exportFilePath = value;

				Modified = true;
				NotifyPropertyChanged("ExportFilePath");
				NotifyPropertyChanged("AcceptEnabled");
			}
		}

		public bool OptionWysiwyg
		{
			get
			{
				return _exportOption == ExportOption.Wysiwyg;
			}
			set
			{
				if (!value)
					_exportOption = ExportOption.CompleteImage;
			}
		}

		public bool OptionCompleteImage
		{
			get
			{
				return _exportOption == ExportOption.CompleteImage;
			}
			set
			{
				if (!value)
					_exportOption = ExportOption.Wysiwyg;
			}
		}

		public float MinimumScale
		{
			get { return 0.1F; }
		}

		public float MaximumScale
		{
			get { return 25F; }
		}

		public float Scale
		{
			get { return _scale; }
			set
			{
				if (value == _scale)
					return;

				_scale = value;
				NotifyPropertyChanged("Scale");
			}
		}

		public bool AcceptEnabled
		{
			get { return Modified; }
		}

		public bool ConfigureEnabled
		{
			get
			{
				if (_selectedImageExporter == null)
					return false;

				return _selectedImageExporter.IsConfigurable;
			}
		}

		public bool ConfigureVisible
		{
			get
			{
				return CollectionUtils.Contains(_imageExporters,
				                                delegate(ImageExporterInfo info)
				                                	{
				                                		return info.IsConfigurable;
				                                	});
			}	
		}

		public void Configure()
		{
			IConfigurableImageExporter exporter = SelectedExporter as IConfigurableImageExporter;

			if (exporter == null)
				return;

			try
			{
				IApplicationComponent component = exporter.GetConfigurationComponent();
				if (component == null)
					return;

				string title = String.Format("{0} ({1})", SR.ConfigureImageExport, exporter.Description);
				LaunchAsDialog(Host.DesktopWindow, component, title);
			}
			catch(Exception e)
			{
				Platform.Log(LogLevel.Error, e);
				Host.DesktopWindow.ShowMessageBox(SR.MessageErrorLaunchingConfigurationComponent, MessageBoxActions.Ok);
			}
		}

		public void Accept()
		{
			if (HasValidationErrors)
			{
				ShowValidation(true);
			}
			else
			{
				UpdateDefaults();
				_exportFilePath = GetCorrectedExportFilePath();

				ExitCode = ApplicationComponentExitCode.Accepted;
				Host.Exit();
			}
		}

		public void Cancel()
		{
			ExitCode = ApplicationComponentExitCode.None;
			Host.Exit();
		}

		#endregion
		
		private void LoadExporters()
		{
			_imageExporters = CollectionUtils.Map<object, ImageExporterInfo>
			(
				new ImageExporterExtensionPoint().CreateExtensions(),
				delegate(object extension)
				{
					return new ImageExporterInfo((IImageExporter)extension);
				}
			);

			_imageExporters.AddRange(GetDefaultExporters());

			SortExporters();
		}

		private IEnumerable<ImageExporterInfo> GetDefaultExporters()
		{
			List<IImageExporter> defaultExporters = StandardImageExporterFactory.CreateStandardExporters();
			foreach (IImageExporter defaultExporter in defaultExporters)
			{
				if (!CollectionUtils.Contains(_imageExporters,
					delegate(ImageExporterInfo info)
					{
						return info.ImageExporter.Identifier == defaultExporter.Identifier;
					}))
				{
					yield return new ImageExporterInfo(defaultExporter);
				}
			}
		}

		private void SortExporters()
		{
			_imageExporters.Sort(delegate(ImageExporterInfo x, ImageExporterInfo y)
						{
							return String.Compare(x.Description, y.Description);
						});
		}

		private string GetCorrectedExportFilePath()
		{
			if (_numberOfImagesToExport > 1)
				return _exportFilePath;

			return FileUtilities.CorrectFileNameExtension(_exportFilePath, SelectedExporter.FileExtensions);
		}

		private void SetDefaults()
		{
			_exportOption = (ExportOption)ImageExportSettings.Default.SelectedImageExportOption;

			_selectedImageExporter = CollectionUtils.SelectFirst(_imageExporters,
											delegate(ImageExporterInfo info)
											{
												return info.ImageExporter.Identifier ==
													ImageExportSettings.Default.SelectedImageExporterId;
											});

			if (_selectedImageExporter == null)
				_selectedImageExporter = _imageExporters[0];
		}

		private void UpdateDefaults()
		{
			ImageExportSettings.Default.SelectedImageExportOption = (int)_exportOption;
			ImageExportSettings.Default.SelectedImageExporterId = SelectedExporter.Identifier;
			ImageExportSettings.Default.Save();
		}

		#endregion

		internal static void Launch(IDesktopWindow desktopWindow, List<IClipboardItem> clipboardItems)
		{
			Platform.CheckForNullReference(clipboardItems, "clipboardItems");

			if (_progressComponentShelf != null)
			{
				desktopWindow.ShowMessageBox(SR.MessageImageExportStillRunning, MessageBoxActions.Ok);
				return;
			}

			int numberOfImagesToExport = GetNumberOfImagesToExport(clipboardItems);
			Platform.CheckPositive(numberOfImagesToExport, "numberOfImagesToExport");

			string title = SR.TitleExportImages;
			if (numberOfImagesToExport == 1)
				title = SR.TitleExportSingleImage;

			ImageExportComponent component = new ImageExportComponent(clipboardItems, numberOfImagesToExport);
			if (ApplicationComponentExitCode.Accepted !=
				ApplicationComponent.LaunchAsDialog(desktopWindow, component, title))
			{
				return;
			}

			component.Export();
		}

		#region Export

		private void Export()
		{
			if (SelectedExporter == null)
				throw new InvalidOperationException("No exporter was chosen; unable to export any images.");

			if (NumberOfImagesToExport == 1)
			{
				if (!Directory.Exists(Path.GetDirectoryName(ExportFilePath ?? "")))
					throw new FileNotFoundException("The specified export directory does not exist.");

				ClipboardItem clipboardItem = (ClipboardItem) _itemsToExport[0];

				ExportImageParams exportParams = new ExportImageParams();
				exportParams.ExportOption = _exportOption;
				exportParams.DisplayRectangle = clipboardItem.DisplayRectangle;
				exportParams.Scale = Scale;
				SelectedExporter.Export((IPresentationImage)clipboardItem.Item, ExportFilePath, exportParams);
			}
			else
			{
				if (!Directory.Exists(ExportFilePath ?? ""))
					throw new FileNotFoundException("The specified export directory does not exist.");


				if (NumberOfImagesToExport <= 5)
				{
					BlockingOperation.Run(delegate { ExportMultipleImages(null); });
					Cleanup(false);
				}
				else
				{
					_itemsToExport.ForEach(delegate(IClipboardItem item) { item.Lock(); });

					BackgroundTask task = new BackgroundTask(ExportMultipleImages, true);

					ProgressDialogComponent progressComponent = 
						new ProgressDialogComponent(task, true, ProgressBarStyle.Blocks);
					_progressComponentShelf = ApplicationComponent.LaunchAsShelf(
						this.Host.DesktopWindow,
						progressComponent, 
						SR.TitleExportingImages, "ExportingImages",
						ShelfDisplayHint.DockFloat);

					_progressComponentShelf.Closed +=
						delegate
							{
								Cleanup(true);
								task.Dispose();
							};
				}
			}
		}

		#region Shared Sync/Async Export method

		private void ExportMultipleImages(IBackgroundTaskContext context)
		{
			int progress = 0;
			List<IPresentationImage> imagesToDispose = new List<IPresentationImage>();

			try
			{
				MultipleImageExportFileNamingStrategy fileNamingStrategy = 
					new MultipleImageExportFileNamingStrategy(ExportFilePath);

				string fileExtension = SelectedExporter.FileExtensions[0];
				ReportProgress(context, SR.MessageExportingImages, progress);

				foreach (ClipboardItem clipboardItem in _itemsToExport)
				{
					if (context != null && context.CancelRequested)
						break;

					ExportImageParams exportParams = new ExportImageParams();
					exportParams.ExportOption = _exportOption;
					exportParams.DisplayRectangle = clipboardItem.DisplayRectangle;
					exportParams.Scale = Scale;
					if (clipboardItem.Item is IPresentationImage)
					{
						IPresentationImage image = (IPresentationImage) clipboardItem.Item;
						if (context != null)
						{
							// A graphic should belong to (and be disposed on) a single thread 
							// and should not be rendered on multiple threads, so we clone it.
							// Technically, we should be doing the clone on the main thread
							// and then passing it to the worker, but that would require blocking
							// the main thread while we cloned all the images.
							imagesToDispose.Add(image = image.Clone());
						}

						string fileName = fileNamingStrategy.GetSingleImageFileName(image, fileExtension);
						SelectedExporter.Export(image, fileName, exportParams);

						ReportProgress(context, fileName, ++progress);
					}
					else if (clipboardItem.Item is IDisplaySet)
					{
						IDisplaySet displaySet = (IDisplaySet) clipboardItem.Item;
						foreach (ImageFileNamePair pair in fileNamingStrategy.GetImagesAndFileNames(displaySet, fileExtension))
						{
							if (context != null && context.CancelRequested)
								break;

							IPresentationImage image = pair.Image;
							if (context != null)
							{
								// A graphic should belong to (and be disposed on) a single thread 
								// and should not be rendered on multiple threads, so we clone it.
								// Technically, we should be doing the clone on the main thread
								// and then passing it to the worker, but that would require blocking
								// the main thread while we cloned all the images.
								imagesToDispose.Add(image = image.Clone());
							}

							SelectedExporter.Export(image, pair.FileName, exportParams);

							ReportProgress(context, pair.FileName, ++progress);
						}
					}
				}

				if (context != null)
				{
					if (context.CancelRequested)
					{
						ReportProgress(context, SR.MessageCancelled, progress);
						context.Cancel();
					}
					else
					{
						ReportProgress(context, SR.MessageExportComplete, progress);
						context.Complete();
					}
				}
			}
			catch(Exception e)
			{
				_error = e;
				Platform.Log(LogLevel.Error, e);
			}
			finally
			{
				imagesToDispose.ForEach(delegate (IPresentationImage image) { image.Dispose(); });
			}
		}

		private void ReportProgress(IBackgroundTaskContext context, string message, int currentStep)
		{
			if (context == null)
				return;

			int percent = Math.Min((int)(currentStep/(float) NumberOfImagesToExport * 100), 100);
			context.ReportProgress(new BackgroundTaskProgress(percent, message));
		}

		#endregion

		private void Cleanup(bool asynchronous)
		{
			if (asynchronous)
				_itemsToExport.ForEach(delegate(IClipboardItem item) { item.Unlock(); });

			_progressComponentShelf = null;

			if (_error != null)
			{
				this.Host.DesktopWindow.ShowMessageBox(SR.MessageExportFailed, MessageBoxActions.Ok);
				_error = null;
			}
		}

		private static int GetNumberOfImagesToExport(IEnumerable<IClipboardItem> itemsToExport)
		{
			int number = 0;
			foreach (ClipboardItem clipboardItem in itemsToExport)
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

		#endregion
	}
}
