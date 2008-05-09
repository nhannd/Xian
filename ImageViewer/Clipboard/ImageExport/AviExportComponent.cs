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

#pragma warning disable 0419,1574,1587,1591

using System.Collections.Generic;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using System;

namespace ClearCanvas.ImageViewer.Clipboard.ImageExport
{
	[ExtensionPoint]
	public sealed class AviExportComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	[AssociateView(typeof(AviExportComponentViewExtensionPoint))]
	public class AviExportComponent : ApplicationComponent
	{
		private static IShelf _progressComponentShelf;

		private volatile ClipboardItem _clipboardItem;
		private volatile string _filePath;
		private volatile int _frameRate = 20;
		private volatile ExportOption _exportOption;
		private volatile float _scale = 1;
		
		private volatile Exception _error;

		private AviExportComponent(ClipboardItem clipboardItem)
		{
			_clipboardItem = clipboardItem;
		}

		private IDisplaySet DisplaySet
		{
			get { return _clipboardItem.Item as IDisplaySet; }	
		}

		private int NumberOfImages
		{
			get { return DisplaySet.PresentationImages.Count; }	
		}

		public override void Start()
		{
			_exportOption = (ExportOption)ImageExportSettings.Default.SelectedVideoExportOption;
			_frameRate = ImageExportSettings.Default.SelectedVideoExportFrameRate;

			base.Start();
		}

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

		public void Accept()
		{
			if (HasValidationErrors)
			{
				ShowValidation(true);
			}
			else
			{
				ImageExportSettings.Default.SelectedVideoExportOption = (int) _exportOption;
				ImageExportSettings.Default.SelectedVideoExportFrameRate = _frameRate;
				ImageExportSettings.Default.Save();

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
			if (ApplicationComponentExitCode.Accepted !=
				ApplicationComponent.LaunchAsDialog(desktopWindow, component, SR.TitleExportToVideo))
			{
				return;
			}

			component.Export();
		}

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

				ProgressDialogComponent progressComponent = 
					new ProgressDialogComponent(task, true, ProgressBarStyle.Blocks);
				_progressComponentShelf = ApplicationComponent.LaunchAsShelf(
					this.Host.DesktopWindow,
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

			try
			{
				ExportImageParams exportParams = new ExportImageParams();
				exportParams.Scale = Scale;
				exportParams.ExportOption = _exportOption;
				exportParams.DisplayRectangle = _clipboardItem.DisplayRectangle;

				ReportProgress(context, SR.MessageCreatingVideo, 0);

				using (AviVideoStreamWriter writer = new AviVideoStreamWriter())
				{
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
								writer.Width = bitmap.Width;
								writer.Height = bitmap.Height;
								writer.FrameRate = this.FrameRate;

								writer.Open(this.FilePath);
							}

							writer.AddBitmap(bitmap);
						}
					}
				}

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

			int percent = Math.Min((int)(currentStep / (float)NumberOfImages * 100), 100);
			context.ReportProgress(new BackgroundTaskProgress(percent, message));
		}
	}
}
