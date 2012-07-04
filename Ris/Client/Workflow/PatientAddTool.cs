#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using System.Threading;
using ClearCanvas.Ris.Application.Common;
using AuthorityTokens = ClearCanvas.Ris.Application.Common.AuthorityTokens;

namespace ClearCanvas.Ris.Client.Workflow
{
	[ButtonAction("apply", "folderexplorer-items-toolbar/New Patient", "Apply")]
	[MenuAction("apply", "folderexplorer-items-contextmenu/New Patient", "Apply")]
	[ButtonAction("apply", "patientsearch-items-toolbar/New Patient", "Apply")]
	[MenuAction("apply", "patientsearch-items-contextmenu/New Patient", "Apply")]
	[Tooltip("apply", "Create a new patient record")]
	[IconSet("apply", IconScheme.Colour, "Icons.AddPatientToolSmall.png", "Icons.AddPatientToolMedium.png", "Icons.AddPatientToolLarge.png")]
	[ActionPermission("apply", ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Patient.Create)]

	[ExtensionOf(typeof(RegistrationWorkflowItemToolExtensionPoint))]
	[ExtensionOf(typeof(PatientSearchToolExtensionPoint))]
	public class PatientAddTool : Tool<IToolContext>
	{
		/// <summary>
		/// Called by the framework when the user clicks the "apply" menu item or toolbar button.
		/// </summary>
		public void Apply()
		{
			if (this.Context is IRegistrationWorkflowItemToolContext)
				Open(((IRegistrationWorkflowItemToolContext)this.Context).DesktopWindow);
			else if (this.Context is IPatientSearchToolContext)
				Open(((IPatientSearchToolContext)this.Context).DesktopWindow);
		}

		private void Open(IDesktopWindow desktopWindow)
		{
			try
			{
				var editor = new PatientProfileEditorComponent();
				var result = ApplicationComponent.LaunchAsDialog(
					desktopWindow,
					editor,
					SR.TitleNewPatient);

				if (result == ApplicationComponentExitCode.Accepted && this.Context is IRegistrationWorkflowItemToolContext)
				{
					// if patient successfully added, invoke a search on the MRN so that the patient appears in the Home page
					var searchParams = new WorklistSearchParams(new WorklistItemTextQueryRequest.AdvancedSearchFields()
																	{
																		Mrn = editor.PatientProfile.Mrn.Id
																	});
					((IRegistrationWorkflowItemToolContext)this.Context).ExecuteSearch(searchParams);
				}
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, desktopWindow);
			}
		}
	}
}
