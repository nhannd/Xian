#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
using System;

namespace ClearCanvas.Ris.Client.Workflow
{
	class PreliminaryDiagnosis
	{
		public static void SetCurrent(ReportingWorklistItemSummary worklistItem, IDesktopWindow desktopWindow)
		{
			Clear();

			Current = new PreliminaryDiagnosis(worklistItem, desktopWindow);
		}

		public static void Clear()
		{
			if (Current != null)
			{
				Current.CloseDialog();
			}
			Current = null;
		}

		public static PreliminaryDiagnosis Current { get; private set; }


		private readonly ReportingWorklistItemSummary _worklistItem;
		private readonly IDesktopWindow _window;
		private Shelf _shelf;
		private bool _completed;
		private bool? _dialogNeeded;

		private PreliminaryDiagnosis(ReportingWorklistItemSummary worklistItem, IDesktopWindow window)
		{
			_worklistItem = worklistItem;
			_window = window;
		}

		public bool IsDialogNeeded()
		{
			if (_completed)
				return false;

			if(!_dialogNeeded.HasValue)
			{
				_dialogNeeded = CheckIfDialogNeeded();
			}
			return _dialogNeeded.Value;
		}


		public void OpenDialogModeless(Action<object> onCompletion)
		{
			// don't open the shelf twice
			if(_shelf != null)
				return;

			// show dialog
			var component = CreateComponent();
			_shelf = ApplicationComponent.LaunchAsShelf(_window, component, MakeTitle(), ShelfDisplayHint.DockFloat);
			_shelf.Closed += delegate
			                 {
			                 	_completed = (component.ExitCode == ApplicationComponentExitCode.Accepted);
			                 	_shelf = null;
								 if(_completed)
								 {
								 	onCompletion(null);
								 }
			                 };
		}

		public ApplicationComponentExitCode OpenDialogModal()
		{
			return ApplicationComponent.LaunchAsDialog(_window, CreateComponent(), MakeTitle());
		}

		public bool IsOpen
		{
			get { return _shelf != null;}
		}

		public bool IsCompleted
		{
			get { return _completed; }
		}

		public void CloseDialog()
		{
			if(_shelf != null)
			{
				_shelf.Close();
				_shelf = null;
			}
		}

		private bool CheckIfDialogNeeded()
		{
			var existingConv = ConversationExists(_worklistItem.OrderRef);

			// if no existing conversation, may not need to show the dialog
			if (!existingConv)
			{
				// if this is not an emergency order, do not show the dialog
				if (!IsEmergencyOrder(_worklistItem.PatientClass.Code))
					return false;

				// otherwise, ask the user if they would like to initiate a PD review
				var msg = string.Format(SR.MessageQueryPrelimDiagnosisReviewRequired, _worklistItem.PatientClass.Value);
				var action = _window.ShowMessageBox(msg, MessageBoxActions.YesNo);
				if (action == DialogBoxAction.No)
					return false;
			}
			return true;
		}

		private OrderNoteConversationComponent CreateComponent()
		{
			return new OrderNoteConversationComponent(_worklistItem.OrderRef, OrderNoteCategory.PreliminaryDiagnosis.Key,
													  PreliminaryDiagnosisSettings.Default.VerificationTemplatesXml,
													  PreliminaryDiagnosisSettings.Default.VerificationSoftKeysXml);
		}

		private string MakeTitle()
		{
			return string.Format(SR.FormatTitleContextDescriptionReviewOrderNoteConversation,
								 PersonNameFormat.Format(_worklistItem.PatientName),
								 MrnFormat.Format(_worklistItem.Mrn),
								 AccessionFormat.Format(_worklistItem.AccessionNumber));
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
