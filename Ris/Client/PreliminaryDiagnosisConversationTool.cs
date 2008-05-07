using System.Collections.Generic;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Extends <see cref="OrderNoteConversationTool{TSummaryItem,TToolContext}"/> to provide a base class for tools which open an 
	/// <see cref="OrderNoteConversationComponent"/> for the purpose of creating a preliminary diagnosis
	/// </summary>
	/// <typeparam name="TSummaryItem"></typeparam>
	/// <typeparam name="TToolContext"></typeparam>
	[MenuAction("pd", "folderexplorer-items-contextmenu/Preliminary Diagnosis", "Open")]
	[ButtonAction("pd", "folderexplorer-items-toolbar/Preliminary Diagnosis", "Open")]
	[Tooltip("pd", "Create/view the preliminary diagnosis for the selected item")]
	[IconSet("pd", IconScheme.Colour, "Icons.PreliminaryDiagnosisToolSmall.png", "Icons.PreliminaryDiagnosisToolSmall.png", "Icons.PreliminaryDiagnosisToolSmall.png")]
	public abstract class PreliminaryDiagnosisConversationTool<TSummaryItem, TToolContext> : OrderNoteConversationTool<TSummaryItem, TToolContext>
		where TSummaryItem : DataContractBase
		where TToolContext : IWorkflowItemToolContext<TSummaryItem>
	{
		protected override IEnumerable<string> OrderNoteCategories
		{
			get { return new string[] { OrderNoteCategory.PreliminaryDiagnosis.Key }; }
		}
		
	}
}