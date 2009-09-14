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

namespace ClearCanvas.Ris.Client.Workflow
{
    static class PreliminaryDiagnosis
    {
        /// <summary>
        /// Gets a value indicating whether a preliminary diagnosis dialog should appear.
        /// </summary>
        /// <param name="orderRef"></param>
		/// <param name="patientClassCode"></param>
		/// <returns></returns>
        public static bool ShouldShowDialog(EntityRef orderRef, string patientClassCode)
        {
            var show = false;
            var filters = new List<string>(new [] { OrderNoteCategory.PreliminaryDiagnosis.Key });
            Platform.GetService<IOrderNoteService>(
            	service => show = service.GetConversation(new GetConversationRequest(orderRef, filters, true)).NoteCount > 0);

			var patientClassFilters = ReportingSettings.Default.PreliminaryDiagnosisReviewForPatientClass;
			var patientClassCodesForReview = string.IsNullOrEmpty(patientClassFilters)
				? new List<string>()
				: CollectionUtils.Map(patientClassFilters.Split(','), (string s) => s.Trim());

			if (patientClassCodesForReview.Contains(patientClassCode))
				show = true;

			return show;
        }

        /// <summary>
        /// Displays the prelminary diagnosis conversation dialog.
        /// </summary>
        /// <param name="orderRef"></param>
        /// <param name="title"></param>
        /// <param name="desktopWindow"></param>
        /// <param name="initialNoteText"></param>
        /// <returns></returns>
        public static ApplicationComponentExitCode ShowConversationDialog(EntityRef orderRef, string title, IDesktopWindow desktopWindow, string initialNoteText)
        {
            var component = new OrderNoteConversationComponent(orderRef, OrderNoteCategory.PreliminaryDiagnosis.Key, null)
                            	{Body = initialNoteText};
        	return ApplicationComponent.LaunchAsDialog(desktopWindow, component, title);
        }
    }
}
