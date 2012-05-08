#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.ServiceModel;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
	[ButtonAction("activate", "dicomstudybrowser-toolbar/ToolbarRetrieveStudy", "RetrieveStudy")]
	[MenuAction("activate", "dicomstudybrowser-contextmenu/MenuRetrieveStudy", "RetrieveStudy")]
    [ActionFormerly("activate", "ClearCanvas.ImageViewer.Services.Tools.RetrieveStudyTool:activate")]
    [EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[Tooltip("activate", "TooltipRetrieveStudy")]
	[IconSet("activate", "Icons.RetrieveStudyToolSmall.png", "Icons.RetrieveStudyToolSmall.png", "Icons.RetrieveStudyToolSmall.png")]
    [ViewerActionPermission("activate", ImageViewer.Common.AuthorityTokens.Study.Retrieve)]
	[ExtensionOf(typeof(StudyBrowserToolExtensionPoint))]
	public class RetrieveStudyTool : StudyBrowserTool
	{
		public override void Initialize()
		{
			base.Initialize();

			SetDoubleClickHandler();
		}

		private void RetrieveStudy()
		{
		    //TODO (Marmot):Restore.
         
            if (!Enabled || Context.SelectedServers.IsLocalServer || Context.SelectedStudy == null)
                return;

            try
            {
                var client = new DicomRetrieveBridge();

                foreach (StudyTableItem study in Context.SelectedStudies)
                {
                    client.RetrieveStudy(study.Server, study);
                    if (Context.SelectedStudies.Count == 1)
                    {
                        DateTime? studyDate = DateParser.Parse(study.StudyDate);
                        Context.DesktopWindow.ShowAlert(AlertLevel.Info,
                                                        string.Format(SR.MessageFormatRetrieveStudyScheduled,
                                                                      study.Server.Name,
                                                                      study.PatientsName.FormattedName,
                                                                      studyDate.HasValue
                                                                          ? Format.Date(studyDate.Value)
                                                                          : string.Empty,
                                                                      study.AccessionNumber),
                                                        SR.LinkOpenActivityMonitor, ActivityMonitorManager.Show);
                    }
                }

                if (Context.SelectedStudies.Count > 1)
                {
                    Context.DesktopWindow.ShowAlert(AlertLevel.Info, string.Format(SR.MessageFormatRetrieveStudiesScheduled, Context.SelectedStudies.Count),
                                                   SR.LinkOpenActivityMonitor, ActivityMonitorManager.Show);
                }
            }
            catch (EndpointNotFoundException)
            {
                 Context.DesktopWindow.ShowMessageBox(SR.MessageRetrieveDicomServerServiceNotRunning, MessageBoxActions.Ok);
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, SR.MessageFailedToRetrieveStudy, Context.DesktopWindow);
            }        
		}

		private bool GetAtLeastOneServerSupportsLoading()
		{
            return Context.SelectedServers.AnySupport<IStudyLoader>();
        }

		private void SetDoubleClickHandler()
		{
			if (!GetAtLeastOneServerSupportsLoading() && Context.SelectedServers.Count > 0)
				Context.DefaultActionHandler = RetrieveStudy;
		}

		protected override void OnSelectedStudyChanged(object sender, EventArgs e)
		{
			UpdateEnabled();
		}
		
		protected override void OnSelectedServerChanged(object sender, EventArgs e)
		{
			UpdateEnabled();
			SetDoubleClickHandler();
		}

		private void UpdateEnabled()
		{
			Enabled = Context.SelectedStudies.Count > 0
                        && !Context.SelectedServers.IsLocalServer
                        && WorkItemActivityMonitor.IsRunning;
    	}
	}
}
