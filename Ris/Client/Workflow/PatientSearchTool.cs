#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Serialization;
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
