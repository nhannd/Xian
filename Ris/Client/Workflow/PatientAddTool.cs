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
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using System.Threading;
using AuthorityTokens=ClearCanvas.Ris.Application.Common.AuthorityTokens;

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
	[ExtensionOf(typeof(BookingWorkflowItemToolExtensionPoint))]
	[ExtensionOf(typeof(PatientSearchToolExtensionPoint))]
	public class PatientAddTool : Tool<IToolContext>
    {
        /// <summary>
        /// Called by the framework when the user clicks the "apply" menu item or toolbar button.
        /// </summary>
        public void Apply()
        {
			if(this.Context is IRegistrationWorkflowItemToolContext)
				Open(((IRegistrationWorkflowItemToolContext)this.Context).DesktopWindow);
			else if (this.Context is IPatientSearchToolContext)
				Open(((IPatientSearchToolContext)this.Context).DesktopWindow);
		}

		private void Open(IDesktopWindow desktopWindow)
		{
			try
			{
				PatientProfileEditorComponent editor = new PatientProfileEditorComponent();
				ApplicationComponentExitCode result = ApplicationComponent.LaunchAsDialog(
					desktopWindow,
					editor,
					SR.TitleNewPatient);

				if (result == ApplicationComponentExitCode.Accepted && 
					Thread.CurrentPrincipal.IsInRole(ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.PatientBiography.View))
				{
					// open the patient overview for the newly created patient
					Document doc = new PatientBiographyDocument(editor.PatientRef, editor.PatientProfileRef, desktopWindow);
					doc.Open();
				}
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, desktopWindow);
			}
		}
    }
}
