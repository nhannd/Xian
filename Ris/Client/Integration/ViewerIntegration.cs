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

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Configuration;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Ris.Client;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer;

namespace ClearCanvas.Ris.Client.Integration
{
	[ExtensionOf(typeof(ViewerIntegrationExtensionPoint))]
    public class ViewerIntegration : IViewerIntegration
	{
		public ViewerIntegration()
        {
        }

        #region IViewerIntegration Members

		public void OpenStudy(string accessionNumber)
		{
			foreach (DesktopWindow window in Application.DesktopWindows)
			{
				foreach (Workspace workspace in window.Workspaces)
				{
					IImageViewer viewer = ImageViewerComponent.GetAsImageViewer(workspace);
					if (viewer == null)
						continue;
					
					foreach (Patient patient in viewer.StudyTree.Patients.Values)
					{
						foreach (Study study in patient.Studies.Values)
						{
							if (study.AccessionNumber == accessionNumber)
							{
								workspace.Activate();
								return;
							}
						}
					}
				}
			}

			StudyItemList studies = FindStudies(accessionNumber);

			if (studies.Count > 0)
			{
				int i = 0;
				string[] uids = new string[studies.Count];
				foreach (StudyItem study in studies)
					uids[i++] = study.StudyInstanceUID;

				OpenStudyHelper.OpenStudies("DICOM_LOCAL", uids, ViewerLaunchSettings.WindowBehaviour);
			}
		}

    	private static StudyItemList FindStudies(string accessionNumber)
        {
			IStudyFinder studyFinder = 
				(IStudyFinder)CollectionUtils.SelectFirst(new StudyFinderExtensionPoint().CreateExtensions(),
        													delegate(object test)
        		                            					{
																	return ((IStudyFinder)test).Name == "DICOM_LOCAL";
        		                            					});

			// do a broad query and choose a random Study
            QueryParameters queryParams = new QueryParameters();
            queryParams.Add("PatientsName", "");
            queryParams.Add("PatientId", "");
            queryParams.Add("AccessionNumber", accessionNumber);
            queryParams.Add("StudyDescription", "");
            queryParams.Add("ModalitiesInStudy", "");
            queryParams.Add("StudyDate", "");
            queryParams.Add("StudyInstanceUid", "");

            StudyItemList accessionStudies = studyFinder.Query(queryParams, null);
			if (accessionStudies == null || accessionStudies.Count == 0)
				return null;

			//return all the studies for the given patient.
    		queryParams["AccessionNumber"] = "";
    		queryParams["PatientId"] = accessionStudies[0].PatientId;
    		return studyFinder.Query(queryParams, null);
        }

		#endregion
    }
}
