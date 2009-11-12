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

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.OrderNotes;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client.Workflow
{
    static class PreliminaryDiagnosis
    {
		/// <summary>
		/// Determines if the PD dialog must be shown upon verification, for the specified worklist item.
		/// </summary>
		/// <param name="worklistItem"></param>
		/// <param name="desktopWindow"></param>
		/// <returns>True if the dialog was shown and accepted, or if it was not required.  False if the user cancelled out of the dialog.</returns>
		public static bool ShowDialogOnVerifyIfRequired(ReportingWorklistItem worklistItem, IDesktopWindow desktopWindow)
		{

			var existingConv = ConversationExists(worklistItem.OrderRef);

			// if no existing conversation, may not need to show the dialog
			if(!existingConv)
			{
				// if this is not an emergency order, do not show the dialog
				if (!IsEmergencyOrder(worklistItem.PatientClass.Code))
					return true;

				// otherwise, ask the user if they would like to initiate a PD review
				var msg = string.Format(SR.MessageQueryPrelimDiagnosisReviewRequired, worklistItem.PatientClass.Value);
				var action = desktopWindow.ShowMessageBox(msg, MessageBoxActions.YesNo);
				if(action == DialogBoxAction.No)
					return true;
			}

			// show dialog
			var title = string.Format(SR.FormatTitleContextDescriptionReviewOrderNoteConversation,
				PersonNameFormat.Format(worklistItem.PatientName),
				MrnFormat.Format(worklistItem.Mrn),
				AccessionFormat.Format(worklistItem.AccessionNumber));

			var component = new OrderNoteConversationComponent(worklistItem.OrderRef, OrderNoteCategory.PreliminaryDiagnosis.Key,
															   PreliminaryDiagnosisSettings.Default.VerificationTemplatesXml,
															   PreliminaryDiagnosisSettings.Default.VerificationSoftKeysXml);

        	return ApplicationComponent.LaunchAsDialog(desktopWindow, component, title) == ApplicationComponentExitCode.Accepted;
		}

		private static bool ConversationExists(EntityRef orderRef)
		{
			var filters = new List<string>(new[] { OrderNoteCategory.PreliminaryDiagnosis.Key });
			var show = false;
			Platform.GetService<IOrderNoteService>(
				service => show = service.GetConversation(new GetConversationRequest(orderRef, filters, true)).NoteCount > 0);

			return show;
		}

		private static bool IsEmergencyOrder(string patientClassCode)
		{
			var patientClassFilters = ReportingSettings.Default.PreliminaryDiagnosisReviewForPatientClass;
			var patientClassCodesForReview = string.IsNullOrEmpty(patientClassFilters)
												? new List<string>()
												: CollectionUtils.Map(patientClassFilters.Split(','), (string s) => s.Trim());

			return patientClassCodesForReview.Contains(patientClassCode);
		}

	}
}
