#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Adt
{
    [MenuAction("launch", "global-menus/Patient/Search Patient")]
    [ButtonAction("launch", "global-toolbars/Patient/Search Patient")]
    [Tooltip("launch", "Search Patient")]
    [IconSet("launch", IconScheme.Colour, "Icons.SearchToolSmall.png", "Icons.SearchToolMedium.png", "Icons.SearchToolLarge.png")]
    [ClickHandler("launch", "Launch")]
    [ExtensionOf(typeof(DesktopToolExtensionPoint))]
    public class PatientSearchTool : Tool<IDesktopToolContext>
    {
        private IWorkspace _workspace;

        public void Launch()
        {
            if (_workspace == null)
            {
                _workspace = ApplicationComponent.LaunchAsWorkspace(
                    this.Context.DesktopWindow,
                    BuildComponent(),
                    SR.TitleSearchPatients,
                    delegate { _workspace = null; });
            }
            else
            {
                _workspace.Activate();
            }
        }

        private static IApplicationComponent BuildComponent()
        {
            PatientSearchComponent searchComponent = new PatientSearchComponent();
            RegistrationPreviewComponent previewComponent = new RegistrationPreviewComponent();

            searchComponent.SelectedProfileChanged += delegate
            {
                PatientProfileSummary summary = (PatientProfileSummary) searchComponent.SelectedProfile.Item;
                if (summary == null)
                    previewComponent.WorklistItem = null;
                else
                    previewComponent.WorklistItem = new RegistrationWorklistItem(
                        summary.ProfileRef,
                        null,
                        summary.Mrn.Id,
                        summary.Mrn.AssigningAuthority,
                        summary.Name,
                        summary.Healthcard,
                        summary.DateOfBirth,
                        summary.Sex,
                        null, null, null, null);
            };

            SplitComponentContainer splitComponent = new SplitComponentContainer(SplitOrientation.Vertical);
            splitComponent.Pane1 = new SplitPane("Search", searchComponent, 1.0f);
            splitComponent.Pane2 = new SplitPane("Preview", previewComponent, 1.0f);

            return splitComponent;
        }
    }
}
