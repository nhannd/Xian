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

using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Workflow
{
	[MenuAction("launch", "folderexplorer-items-contextmenu/Patient Search", "Launch")]
	[IconSet("launch", IconScheme.Colour, "Icons.SearchPatientToolSmall.png", "Icons.SearchPatientToolMedium.png", "Icons.SearchPatientToolLarge.png")]
	[Tooltip("launch", "Patient Search")]

	[ExtensionOf(typeof(RegistrationWorkflowItemToolExtensionPoint))]
	public class PatientSearchTool : Tool<IRegistrationWorkflowItemToolContext>
	{
		class PreviewComponent : DHtmlComponent
		{
			private PatientProfileSummary _patientProfile;

			public PatientProfileSummary PatientProfile
			{
				get { return _patientProfile; }
				set
				{
					_patientProfile = value;
					this.SetUrl(WebResourcesSettings.Default.RegistrationFolderSystemUrl);
				}
			}

			protected override ActionModelNode GetActionModel()
			{
				return new ActionModelRoot();
			}

			protected override DataContractBase GetHealthcareContext()
			{
				return _patientProfile;
			}

		}


		private IWorkspace _workspace;

		public void Launch()
		{
			try
			{
				if (_workspace == null)
				{
					_workspace = ApplicationComponent.LaunchAsWorkspace(
						this.Context.DesktopWindow,
						BuildComponent(),
						SR.TitlePatientSearch);
					_workspace.Closed += delegate { _workspace = null; };
				}
				else
				{
					_workspace.Activate();
				}
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Context.DesktopWindow);
			}
		}

		private static IApplicationComponent BuildComponent()
		{
			PatientSearchComponent searchComponent = new PatientSearchComponent();
			PreviewComponent previewComponent = new PreviewComponent();

			searchComponent.SelectedProfileChanged += delegate
			{
				previewComponent.PatientProfile = (PatientProfileSummary)searchComponent.SelectedProfile.Item;
			};

			SplitComponentContainer splitComponent = new SplitComponentContainer(SplitOrientation.Vertical);
			splitComponent.Pane1 = new SplitPane("Search", searchComponent, 1.0f);
			splitComponent.Pane2 = new SplitPane("Preview", previewComponent, 1.0f);

			return splitComponent;
		}
	}
}
