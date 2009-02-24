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
            bool show = false;
            List<string> filters = new List<string>(new string[] { OrderNoteCategory.PreliminaryDiagnosis.Key });
            Platform.GetService<IOrderNoteService>(
                delegate(IOrderNoteService service)
                {
					show = service.GetConversation(new GetConversationRequest(orderRef, filters, true)).NoteCount > 0;
                });

			string patientClassFilters = ReportingSettings.Default.PreliminaryDiagnosisReviewForPatientClass;
			List<string> patientClassCodesForReview = string.IsNullOrEmpty(patientClassFilters)
				? new List<string>()
				: CollectionUtils.Map<string, string>(patientClassFilters.Split(','), delegate(string s) { return s.Trim(); });

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
            OrderNoteConversationComponent component = new OrderNoteConversationComponent(orderRef, OrderNoteCategory.PreliminaryDiagnosis.Key);
            component.Body = initialNoteText;
            return ApplicationComponent.LaunchAsDialog(desktopWindow, component, title);
        }
    }
}
