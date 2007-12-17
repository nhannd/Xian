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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.StudyFinders.LocalDataStore;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Ris.Client;

namespace ClearCanvas.Ris.Client.Integration
{
    [ExtensionOf(typeof(ViewerIntegrationExtensionPoint))]
    public class ViewerIntegration : IViewerIntegration
    {
        /// <summary>
        /// No-args constructor required by extension point framework.
        /// </summary>
        public ViewerIntegration()
        {
            if (!Platform.IsWin32Platform)
                throw new NotSupportedException();
        }

        #region IViewerIntegration Members

        public void OpenStudy(string accessionNumber)
        {
            StudyItemList studies = FindStudies(accessionNumber);

            if (studies.Count > 0)
            {
                string windowName = "ImageViewer";

                DesktopWindow viewerWindow =
                    CollectionUtils.SelectFirst<DesktopWindow>(ClearCanvas.Desktop.Application.DesktopWindows,
                    delegate(DesktopWindow window)
                    {
                        return window.Name == windowName;
                    });

                if (viewerWindow == null)
                    viewerWindow = ClearCanvas.Desktop.Application.DesktopWindows.AddNew(windowName);
                else
                    viewerWindow.Activate();

                foreach (StudyItem study in studies)
                {
                    OpenStudy(study, viewerWindow);
                }
            }
        }

        #endregion

        #region Private Helpers

        private static StudyItemList FindStudies(string accessionNumber)
        {
            LocalDataStoreStudyFinder studyFinder = new LocalDataStoreStudyFinder();

            // do a broad query and choose a random Study
            QueryParameters queryParams = new QueryParameters();
            queryParams.Add("PatientsName", "");
            queryParams.Add("PatientId", "");
            queryParams.Add("AccessionNumber", accessionNumber);
            queryParams.Add("StudyDescription", "");
            queryParams.Add("ModalitiesInStudy", "");
            queryParams.Add("StudyDate", "");
            queryParams.Add("StudyInstanceUid", "");

            return studyFinder.Query(queryParams, null);
        }

        private static void OpenStudy(StudyItem study, IDesktopWindow window)
        {
            DiagnosticImageViewerComponent imageViewer = new DiagnosticImageViewerComponent();

            imageViewer.LoadStudy(study.StudyInstanceUID, "DICOM_LOCAL");

            string workspaceName = imageViewer.PatientsLoadedLabel;

            Workspace workspace =
                CollectionUtils.SelectFirst<Workspace>(window.Workspaces,
                delegate(Workspace ws)
                {
                    return ws.Name == workspaceName;
                });

            if (workspace == null)
                window.Workspaces.AddNew(new WorkspaceCreationArgs(imageViewer, workspaceName, workspaceName));

            //ApplicationComponent.LaunchAsWorkspace(
            //    this.Context.DesktopWindow,
            //    imageViewer,
            //    imageViewer.PatientsLoadedLabel,
            //    delegate
            //    {
            //        imageViewer.Dispose();
            //    });

            imageViewer.Layout();
        }

        #endregion
    }
}
