using System.Collections.Generic;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Extends <see cref="OrderNoteConversationToolBase{TSummaryItem,TToolContext}"/> to provide a base class for tools which open an 
	/// <see cref="OrderNoteConversationComponent"/> for the purpose of creating a preliminary diagnosis
	/// </summary>
	/// <typeparam name="TSummaryItem"></typeparam>
	/// <typeparam name="TToolContext"></typeparam>
	[MenuAction("pd", "folderexplorer-items-contextmenu/Preliminary Diagnosis", "Open")]
	[ButtonAction("pd", "folderexplorer-items-toolbar/Preliminary Diagnosis", "Open")]
	[Tooltip("pd", "Create/view the preliminary diagnosis for the selected item")]
	[IconSet("pd", IconScheme.Colour, "Icons.PreliminaryDiagnosisToolSmall.png", "Icons.PreliminaryDiagnosisToolSmall.png", "Icons.PreliminaryDiagnosisToolSmall.png")]
	public abstract class PreliminaryDiagnosisConversationTool<TSummaryItem, TToolContext> : OrderNoteConversationToolBase<TSummaryItem, TToolContext>
		where TSummaryItem : WorklistItemSummaryBase
		where TToolContext : IWorkflowItemToolContext<TSummaryItem>
	{
		protected override EntityRef OrderRef
		{
			get { return this.SummaryItem.OrderRef; }
		}

		protected override string TitleContextDescription
		{
			get
			{
				return string.Format(SR.FormatTitleContextDescriptionOrderNoteConversation,
					PersonNameFormat.Format(this.SummaryItem.PatientName),
					MrnFormat.Format(this.SummaryItem.Mrn),
					AccessionFormat.Format(this.SummaryItem.AccessionNumber));
			}
		}

		protected override IEnumerable<string> OrderNoteCategories
		{
			get { return new string[] { OrderNoteCategory.PreliminaryDiagnosis.Key }; }
		}
		
	}
}