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

//not a production tool right now.
#if DEBUG

using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Explorer.Dicom;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Services.LocalDataStore;

namespace ClearCanvas.Utilities.DicomEditor
{
	[ButtonAction("activate", "dicomstudybrowser-toolbar/ToolbarAnonymizeStudy", "AnonymizeStudy")]
	[MenuAction("activate", "dicomstudybrowser-contextmenu/MenuAnonymizeStudy", "AnonymizeStudy")]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[Tooltip("activate", "TooltipAnonymizeStudy")]
	[IconSet("activate", IconScheme.Colour, "Icons.AnonymizeStudyToolSmall.png", "Icons.AnonymizeStudyToolSmall.png", "Icons.AnonymizeStudyToolSmall.png")]

	//TODO: Remove this later.
	[ActionPermission("activate", "DemoAdmin")]
	[ExtensionOf(typeof(StudyBrowserToolExtensionPoint))]
	public class AnonymizeStudyTool : StudyBrowserTool, IAnonymizationCallback
	{
		private volatile AnonymizeStudyComponent _component;
		private volatile IBackgroundTaskContext _taskContext;

		public AnonymizeStudyTool()
		{
		}

		public void AnonymizeStudy()
		{
			_component = new AnonymizeStudyComponent(this.Context.SelectedStudy);
			if (ApplicationComponentExitCode.Accepted == 
				ApplicationComponent.LaunchAsDialog(this.Context.DesktopWindow, _component, SR.TitleAnonymizeStudy))
			{
				BackgroundTask task = null;
				try
				{
					task = new BackgroundTask(Anonymize, false, this.Context.SelectedStudy.StudyInstanceUID);
					ProgressDialog.Show(task, this.Context.DesktopWindow, true);
				}
				catch(Exception e)
				{
					Platform.Log(LogLevel.Error, e);
					this.Context.DesktopWindow.ShowMessageBox(SR.MessageFormatStudyMustBeDeletedManually, MessageBoxActions.Ok);
				}
				finally
				{
					_taskContext = null;

					if (task != null)
						task.Dispose();
				}
			}
		}

		private void Anonymize(IBackgroundTaskContext context)
		{
			_taskContext = context;

			AnonymizationHelper.AnonymizeLocalStudy(
				(string)context.UserState,
				_component.PatientId,
				_component.PatientsName,
				_component.DateOfBirth,
				_component.AccessionNumber,
				_component.StudyDescription,
				_component.StudyDate,
				true, 
				_component.PreserveSeriesDescriptions, this);
		}

		private void UpdateEnabled()
		{
			if (this.Context.SelectedStudy == null)
			{
				this.Enabled = false;
				return;
			}

			this.Enabled = AnonymizationHelper.CanAnonymizeStudy() && 
							LocalDataStoreActivityMonitor.Instance.IsConnected && 
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

		#region IAnonymizationCallback Members

		void IAnonymizationCallback.BeforeAnonymize(DicomAttributeCollection dataSet)
		{
		}

		void IAnonymizationCallback.BeforeSave(DicomAttributeCollection dataSet)
		{
		}

		void IAnonymizationCallback.ReportProgress(int percent, string message)
		{
			_taskContext.ReportProgress(new BackgroundTaskProgress(percent, message));		
		}

		#endregion
	}
}

#endif