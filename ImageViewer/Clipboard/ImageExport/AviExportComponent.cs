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

#pragma warning disable 0419,1574,1587,1591

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Services.Auditing;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Clipboard.ImageExport
{
	[ExtensionPoint]
	public sealed class AviExportComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	[AssociateView(typeof(AviExportComponentViewExtensionPoint))]
	public class AviExportComponent : ApplicationComponent
	{
		#region Private Fields

		private static IShelf _progressComponentShelf;

		private volatile ClipboardItem _clipboardItem;
		private volatile string _filePath;
		private volatile int _frameRate = 20;
		private volatile ExportOption _exportOption;
		private volatile float _scale = 1;

		private int _width = 512;
		private int _height = 512;
		private Color _backgroundColor = Color.Black;
		private SizeMode _sizeMode = SizeMode.Scale;

		private readonly List<Avi.Codec> _availableCodecs;
		private Avi.Codec _selectedCodec;
		private bool _useDefaultQuality;
		private int _quality;

		private volatile Exception _error;

		#endregion

		private AviExportComponent(ClipboardItem clipboardItem)
		{
			_clipboardItem = clipboardItem;
			_availableCodecs = new List<Avi.Codec>();
		}

		#region Private Properties

		private IDisplaySet DisplaySet
		{
			get { return _clipboardItem.Item as IDisplaySet; }	
		}

		private int NumberOfImages
		{
			get { return DisplaySet.PresentationImages.Count; }	
		}

		private Avi.Codec SelectedCodec
		{
			get { return _selectedCodec; }
		}

		private bool UseDefaultQuality
		{
			get { return _useDefaultQuality; }
			set
			{
				if (!_selectedCodec.SupportsQuality)
					value = true;

				_useDefaultQuality = value;
			}
		}

		#endregion

		#region Presentation Model

		public string FilePath
		{
			get { return _filePath; }
			set
			{
				if (_filePath == value)
					return;

				_filePath = value;
				NotifyPropertyChanged("FilePath");
			}
		}

		public int MinFrameRate
		{
			get { return 1; }
		}

		public int MaxFrameRate
		{
			get { return 25; }
		}

		public int FrameRate
		{
			get { return _frameRate; }
			set
			{
				if (_frameRate == value)
					return;
				
				_frameRate = value;
				NotifyPropertyChanged("FrameRate");
				NotifyPropertyChanged("DurationSeconds");
			}
		}

		public float DurationSeconds
		{
			get { return NumberOfImages / (float)_frameRate; }
		}

		public string FileExtensionFilter
		{
			get { return string.Format("{0}|*.avi", SR.DescriptionAvi); }
		}

		public string DefaultFileExtension
		{
			get { return "avi"; }
		}

		public ExportOption ExportOption
		{
			get { return _exportOption; }
			set
			{
				if (_exportOption != value)
				{
					_exportOption = value;
					this.NotifyPropertyChanged("ExportOption");
				}
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

		public int MinimumDimension
		{
			get { return 10; }
		}

		public int MaximumDimension
		{
			get { return 100000; }
		}

		public int Width
		{
			get { return _width; }
			set
			{
				if (_width != value)
				{
					_width = value;
					this.NotifyPropertyChanged("Width");
				}
			}
		}

		public int Height
		{
			get { return _height; }
			set
			{
				if (_height != value)
				{
					_height = value;
					this.NotifyPropertyChanged("Height");
				}
			}
		}

		public Color BackgroundColor
		{
			get { return _backgroundColor; }
			set
			{
				if (_backgroundColor != value)
				{
					_backgroundColor = value;
					this.NotifyPropertyChanged("BackgroundColor");
				}
			}
		}

		public SizeMode SizeMode
		{
			get { return _sizeMode; }
			set
			{
				if (_sizeMode != value)
				{
					_sizeMode = value;
					this.NotifyPropertyChanged("SizeMode");
				}
			}
		}

		public void ShowAdvanced()
		{
			AviExportAdvancedComponent component = new AviExportAdvancedComponent(_availableCodecs);
			component.SelectedCodec = _selectedCodec;
			component.Quality = _quality;
			component.UseDefaultQuality = UseDefaultQuality;

			if (ApplicationComponentExitCode.Accepted != LaunchAsDialog(this.Host.DesktopWindow, component, SR.TitleAdvancedOptions))
				return;

			_selectedCodec = component.SelectedCodec;
			_quality = component.Quality;
			UseDefaultQuality = component.UseDefaultQuality;

			SaveAdvancedSettings();
		}

		public void Accept()
		{
			if (HasValidationErrors)
			{
				ShowValidation(true);
			}
			else
			{
				SaveSettings();
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

		internal static void Launch(IDesktopWindow desktopWindow, ClipboardItem clipboardItem)
		{
			Platform.CheckForNullReference(clipboardItem, "clipboardItem");
			IDisplaySet displaySet = clipboardItem.Item as IDisplaySet;
			if (displaySet == null)
				throw new ArgumentException("The item being exported must be a display set.");

			if (_progressComponentShelf != null)
			{
				desktopWindow.ShowMessageBox(SR.MessageVideoExportStillRunning, MessageBoxActions.Ok);
				return;
			}

			if (displaySet.PresentationImages.Count <= 1)
			{
				desktopWindow.ShowMessageBox(SR.MessageDisplaySetTooFewImagesForVideo, MessageBoxActions.Ok);
				return;
			}

			AviExportComponent component = new AviExportComponent(clipboardItem);
			component.LoadSettings();
			if (component.SelectedCodec == null)
			{
				desktopWindow.ShowMessageBox(SR.MessageNoAcceptableCodecsInstalled, MessageBoxActions.Ok);
				return;
			}

			// give the width and height values from the item to be exported
			foreach (IPresentationImage image in (displaySet.PresentationImages))
			{
				if (image is IImageGraphicProvider)
				{
					IImageGraphicProvider imageGraphicProvider = (IImageGraphicProvider) image;
					component.Height = imageGraphicProvider.ImageGraphic.Rows;
					component.Width = imageGraphicProvider.ImageGraphic.Columns;
					break;
				}
			}

			if (ApplicationComponentExitCode.Accepted != LaunchAsDialog(desktopWindow, component, SR.TitleExportToVideo))
				return;

			component.Export();
		}

		#region Export

		private void Export()
		{
			if (DisplaySet.PresentationImages.Count <= 10)
			{
				ExportImages(null);
				Cleanup();
			}
			else
			{
				BackgroundTask task = new BackgroundTask(ExportImages, true);

				_clipboardItem.Lock();

				ProgressDialogComponent progressComponent = new ProgressDialogComponent(task, true, ProgressBarStyle.Blocks);
				_progressComponentShelf = ApplicationComponent.LaunchAsShelf(this.Host.DesktopWindow,
																progressComponent,
																SR.TitleCreatingVideo, "CreatingVideo",
																ShelfDisplayHint.DockFloat);

				_progressComponentShelf.Closed +=
					delegate
						{
							_clipboardItem.Unlock();
							Cleanup();
							task.Dispose();
						};
			}
		}

		private void Cleanup()
		{
			_progressComponentShelf = null;
			if (_error != null)
			{
				this.Host.DesktopWindow.ShowMessageBox(SR.MessageExportToVideoFailed, MessageBoxActions.Ok);
				_error = null;
			}
		}

		private void ExportImages(IBackgroundTaskContext context)
		{
			List<IPresentationImage> imagesToDispose = new List<IPresentationImage>();

			EventResult result = EventResult.Success;
			AuditedInstances exportedInstances = new AuditedInstances();
			foreach (IPresentationImage image in this.DisplaySet.PresentationImages)
			{
				IImageSopProvider sopProv = image as IImageSopProvider;
				if (sopProv != null)
					exportedInstances.AddInstance(sopProv.ImageSop.PatientId, sopProv.ImageSop.PatientsName, sopProv.ImageSop.StudyInstanceUid, this.FilePath);
			}

			try
			{
				ReportProgress(context, SR.MessageCreatingVideo, 0);

				ExportImages(context, ref imagesToDispose);

				if (context != null)
				{
					if (context.CancelRequested)
					{
						ReportProgress(context, SR.MessageCancelled, NumberOfImages);
						context.Cancel();
					}
					else
					{
						ReportProgress(context, SR.MessageExportComplete, NumberOfImages);
						context.Complete();
					}
				}
			}
			catch(Exception e)
			{
				_error = e;
				result = EventResult.SeriousFailure;
				Platform.Log(LogLevel.Error, e);
			}
			finally
			{
				AuditHelper.LogExportStudies(exportedInstances, EventSource.CurrentUser, result);
				imagesToDispose.ForEach(delegate(IPresentationImage image) { image.Dispose(); });
			}
		}

		private void ExportImages(IBackgroundTaskContext context, ref List<IPresentationImage> imagesToDispose)
		{
			ExportImageParams exportParams = new ExportImageParams();
			exportParams.Scale = Scale;
			exportParams.ExportOption = _exportOption;
			exportParams.DisplayRectangle = _clipboardItem.DisplayRectangle;
			exportParams.SizeMode = SizeMode;
			exportParams.OutputSize = new Size(Width, Height);
			exportParams.BackgroundColor = BackgroundColor;

			using (Avi.VideoStreamWriter writer = new Avi.VideoStreamWriter(_selectedCodec))
			{
				if (!UseDefaultQuality)
					writer.Quality = _quality;

				for (int i = 0; i < NumberOfImages; ++i)
				{
					if (context != null && context.CancelRequested)
						break;

					if (context != null)
					{
						int number = i + 1;
						string message = String.Format(SR.MessageFormatExportingFrame, number, NumberOfImages);
						ReportProgress(context, message, number);
					}

					IPresentationImage image = DisplaySet.PresentationImages[i].Clone();
					imagesToDispose.Add(image);

					using (Bitmap bitmap = ImageExporter.DrawToBitmap(image, exportParams))
					{
						if (!writer.IsOpen)
						{
							//many codecs (like DivX) expect width and/or height to be divisible by 4.
							int width = NormalizeDimension(bitmap.Width);
							int height = NormalizeDimension(bitmap.Height);

							writer.Width = width;
							writer.Height = height;
							writer.FrameRate = this.FrameRate;

							writer.Open(this.FilePath);
						}

						writer.AddBitmap(bitmap);
					}
				}
			}
		}

		private void ReportProgress(IBackgroundTaskContext context, string message, int currentStep)
		{
			if (context == null)
				return;

			int percent = Math.Min((int)(currentStep / (float)NumberOfImages * 100), 100);
			context.ReportProgress(new BackgroundTaskProgress(percent, message));
		}

		#endregion

		#region Private Methods

		private void LoadSettings() 
		{
			AviExportSettings settings = AviExportSettings.Default;
			_exportOption = (ExportOption)settings.ExportOption;
			_frameRate = settings.FrameRate;

			_selectedCodec = Avi.Codec.GetInstalledCodec(settings.PreferredCodecFccCode);

			_availableCodecs.AddRange(GetAvailiableCodecs());
			if (!_availableCodecs.Contains(_selectedCodec))
				_selectedCodec = Avi.Codec.Find(GetInputFormat(), null); //use whatever the OS provides.

			_quality = settings.Quality;
			UseDefaultQuality = settings.UseDefaultQuality;

			_sizeMode = settings.SizeMode;
			_backgroundColor = settings.BackgroundColor;
		}

		private void SaveSettings()
		{
			AviExportSettings settings = AviExportSettings.Default;
			settings.ExportOption = (int)_exportOption;
			settings.FrameRate = _frameRate;
			settings.SizeMode = SizeMode;
			settings.BackgroundColor = BackgroundColor;
			settings.Save();
		}

		private void SaveAdvancedSettings()
		{
			AviExportSettings settings = AviExportSettings.Default;
			settings.PreferredCodecFccCode = _selectedCodec.FourCCCode;
			settings.UseDefaultQuality = UseDefaultQuality;
			settings.Quality = _quality;
			settings.Save();
		}

		private IEnumerable<Avi.Codec> GetAvailiableCodecs()
		{
			Avi.BITMAPINFOHEADER header = GetInputFormat();

			foreach(Avi.Codec codec in Avi.Codec.GetInstalledCodecs())
			{
				if (codec.CanCompress(header))
					yield return codec;
			}
		}

		private static Avi.BITMAPINFOHEADER GetInputFormat()
		{
			Avi.BITMAPINFOHEADER format = new Avi.BITMAPINFOHEADER();
			format.biSize = Marshal.SizeOf(format);
			format.biPlanes = 1;
			format.biBitCount = 24;
			format.biCompression = 0; //RGB
			format.biHeight = 512; //use 512x512 just to get the available codecs.
			format.biWidth = 512;
			format.biSizeImage = 3*format.biWidth*format.biHeight;
			return format;
		}

		private static int NormalizeDimension(int dimension)
		{
			int offset = dimension%4;
			if (offset == 0)
				return dimension;

			return dimension + (4 - offset);
		}

		#endregion
	}
}
