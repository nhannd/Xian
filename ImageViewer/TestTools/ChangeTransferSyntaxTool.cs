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
using ClearCanvas.Dicom.Codec;

namespace ClearCanvas.ImageViewer.TestTools
{
	//Note: this class was basically copied from the anonymization tool and is *not* the correct
	//way to do this.  It's just a quick and dirty tool for testing purposes.
	//It should really be integrated into the viewer services.  Actually, so should anonymization!

	[ExtensionOf(typeof(StudyBrowserToolExtensionPoint))]
	public class ChangeTransferSyntaxTool : StudyBrowserTool
	{
		private string _tempPath;
		private static object _localStudyLoader = null;
		private TransferSyntax _syntax;

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

		public override IActionSet Actions
		{
			get
			{
				List<IAction> actions = new List<IAction>();
				IResourceResolver resolver = new ResourceResolver(typeof(ChangeTransferSyntaxTool).GetType().Assembly);

				actions.Add(CreateAction(TransferSyntax.ExplicitVrLittleEndian, resolver));
				actions.Add(CreateAction(TransferSyntax.ImplicitVrLittleEndian, resolver));

				foreach (IDicomCodecFactory factory in ClearCanvas.Dicom.Codec.DicomCodecRegistry.GetCodecFactories())
				{
					actions.Add(CreateAction(factory.CodecTransferSyntax, resolver));
				}

				return new ActionSet(actions);
			}
		}

		private IAction CreateAction(TransferSyntax syntax, IResourceResolver resolver)
		{
			ClickAction action = new ClickAction(syntax.UidString,
					new ActionPath("dicomstudybrowser-contextmenu/Change Transfer Syntax/" + syntax.ToString(), resolver),
					ClickActionFlags.None, resolver);
			action.SetClickHandler(delegate { ChangeToSyntax(syntax); });
			action.Label = syntax.ToString();
			return action;
		}

		public void ChangeToSyntax(TransferSyntax syntax)
		{
			_syntax = syntax;

			StudyItem selectedStudy = this.Context.SelectedStudy;

			BackgroundTask task = null;
			try
			{
				task = new BackgroundTask(ChangeToSyntax, false, this.Context.SelectedStudy);
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

		private void ChangeToSyntax(IBackgroundTaskContext context)
		{
			StudyItem study = (StudyItem)context.UserState;

			try
			{
				_tempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "ClearCanvas");
				_tempPath = System.IO.Path.Combine(_tempPath, "Compression");
				_tempPath = System.IO.Path.Combine(_tempPath, Path.GetRandomFileName());

				string message = String.Format("Changing transfer syntax to: {0}", _syntax);
				context.ReportProgress(new BackgroundTaskProgress(0, message));

				int numberOfSops = LocalStudyLoader.Start(new StudyLoaderArgs(study.StudyInstanceUid, null));
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
							file.ChangeTransferSyntax(_syntax);
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