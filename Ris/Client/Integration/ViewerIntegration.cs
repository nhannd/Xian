using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.StudyFinders.LocalDataStore;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Ris.Client;

namespace ClearCanvas.Ris.Integration
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

        private StudyItemList FindStudies(string accessionNumber)
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

        private void OpenStudy(StudyItem study, DesktopWindow window)
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
