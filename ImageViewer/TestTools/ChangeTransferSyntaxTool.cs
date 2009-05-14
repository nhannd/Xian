#if DEBUG

using System;
using System.Collections.Generic;
using System.IO;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Explorer.Dicom;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Services.LocalDataStore;
using ClearCanvas.ImageViewer.StudyManagement;
using Path=System.IO.Path;

namespace ClearCanvas.ImageViewer.TestTools
{
	//Note: this class was basically copied from the anonymization tool and is *not* the correct
	//way to do this.  It's just a quick and dirty tool for testing purposes.
	//It should really be integrated into the viewer services.  Actually, so should anonymization!
	[MenuAction("activate", "dicomstudybrowser-contextmenu/Compress J2K", "CompressJ2K")]
	[ExtensionOf(typeof(StudyBrowserToolExtensionPoint))]
	public class ChangeTransferSyntaxTool : StudyBrowserTool
	{
		private string _tempPath;
		private static object _localStudyLoader = null;
		
		public ChangeTransferSyntaxTool()
		{
		}

		private static IStudyLoader LocalStudyLoader
		{
			get
			{
				if (_localStudyLoader == null)
				{
					try
					{
						StudyLoaderExtensionPoint xp = new StudyLoaderExtensionPoint();
						foreach (IStudyLoader loader in xp.CreateExtensions())
						{
							if (loader.Name == "DICOM_LOCAL")
							{
								_localStudyLoader = loader;
								break;
							}
						}
					}
					catch (NotSupportedException)
					{
						Platform.Log(LogLevel.Info, "Compressions tool disabled; no local study loader exists.");
					}

					if (_localStudyLoader == null)
						_localStudyLoader = new object(); //there is no loader.
				}

				return _localStudyLoader as IStudyLoader;
			}
		}

		public void CompressJ2K()
		{
			StudyItem selectedStudy = this.Context.SelectedStudy;

			BackgroundTask task = null;
			try
			{
				task = new BackgroundTask(CompressJ2K, false, this.Context.SelectedStudy);
				ProgressDialog.Show(task, this.Context.DesktopWindow, true);
			}
			catch(Exception e)
			{
				Platform.Log(LogLevel.Error, e);
				string message = String.Format("An error occurred while compressing; folder must be deleted manually: {0}", _tempPath);
				this.Context.DesktopWindow.ShowMessageBox(message, MessageBoxActions.Ok);
			}
			finally
			{
				_tempPath = null;

				if (task != null)
					task.Dispose();
			}
		}

		private void CompressJ2K(IBackgroundTaskContext context)
		{
			StudyItem study = (StudyItem)context.UserState;

			try
			{
				_tempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "ClearCanvas");
				_tempPath = System.IO.Path.Combine(_tempPath, "Compression");

				context.ReportProgress(new BackgroundTaskProgress(0, "Compressing Study J2K Lossless ..."));

				int numberOfSops = LocalStudyLoader.Start(new StudyLoaderArgs(study.StudyInstanceUID, null));
				if (numberOfSops <= 0)
					return;

				for (int i = 0; i < numberOfSops; ++i)
				{
					Sop sop = LocalStudyLoader.LoadNextSop();
					if (sop != null)
					{
						if (sop.DataSource is ILocalSopDataSource)
						{
							string filename = Path.Combine(_tempPath, string.Format("{0}.dcm", i));
							DicomFile file = ((ILocalSopDataSource)sop.DataSource).File;
							file.ChangeTransferSyntax(TransferSyntax.Jpeg2000ImageCompressionLosslessOnly);
							file.Save(filename);
						}
					}

					int progressPercent = (int)Math.Floor((i + 1) / (float)numberOfSops * 100);
					string progressMessage = "Compressing Study J2K Lossless ...";
					context.ReportProgress(new BackgroundTaskProgress(progressPercent, progressMessage));
				}

				//trigger an import of the anonymized files.
				LocalDataStoreServiceClient client = new LocalDataStoreServiceClient();
				client.Open();
				try
				{
					FileImportRequest request = new FileImportRequest();
					request.BadFileBehaviour = BadFileBehaviour.Move;
					request.FileImportBehaviour = FileImportBehaviour.Move;
					List<string> path = new List<string>();
					path.Add(_tempPath);
					request.FilePaths = path;
					request.Recursive = true;
					request.IsBackground = true;
					client.Import(request);
					client.Close();
				}
				catch
				{
					client.Abort();
					throw;
				}

				context.Complete();
			}
			catch(Exception e)
			{
				context.Error(e);
			}
		}

		private void UpdateEnabled()
		{
			if (this.Context.SelectedStudy == null)
			{
				this.Enabled = false;
				return;
			}

			this.Enabled = LocalStudyLoader != null && 
			               LocalDataStoreActivityMonitor.IsConnected && 
			               this.Context.SelectedStudies.Count == 1 && 
			               this.Context.SelectedServerGroup.IsLocalDatastore;
		}

		protected override void OnSelectedStudyChanged(object sender, EventArgs e)
		{
			UpdateEnabled();
		}

		protected override void OnSelectedServerChanged(object sender, EventArgs e)
		{
			UpdateEnabled();
		}
	}
}

#endif