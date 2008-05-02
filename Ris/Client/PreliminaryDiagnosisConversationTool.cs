using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Opens a <see cref="PreliminaryDiagnosisConversationComponent"/>
	/// </summary>
	/// <typeparam name="TSummaryItem">A <see cref="DataContractBase"/> that can provide an OrderRef and an appropriate value for the component's title</typeparam>
	/// <typeparam name="TToolContext">Must be <see cref="IWorkflowItemToolContext{TSummaryItem}"/></typeparam>
	[MenuAction("pd", "folderexplorer-items-contextmenu/Preliminary Diagnosis", "Open")]
	[ButtonAction("pd", "folderexplorer-items-toolbar/Preliminary Diagnosis", "Open")]
	[Tooltip("pd", "Create/view the preliminary diagnosis for the selected item")]
	[IconSet("pd", IconScheme.Colour, "Icons.PreliminaryDiagnosisToolSmall.png", "Icons.PreliminaryDiagnosisToolSmall.png", "Icons.PreliminaryDiagnosisToolSmall.png")]
	public abstract class PreliminaryDiagnosisConversationTool<TSummaryItem, TToolContext> : Tool<TToolContext> 
		where TSummaryItem : DataContractBase
		where TToolContext : IWorkflowItemToolContext<TSummaryItem>
	{
		/// <summary>
		/// An title for the <see cref="PreliminaryDiagnosisConversationComponent"/> derived-classes <see cref="TSummaryItem"/>
		/// </summary>
		protected abstract string Title { get; }

		/// <summary>
		/// A <see cref="EntityRef"/> for an Order from the derived-classes <see cref="TSummaryItem"/>.
		/// </summary>
		protected abstract EntityRef OrderRef { get; }

		/// <summary>
		/// The first <see cref="TSummaryItem"/> in the current <see cref="TToolContext"/>
		/// </summary>
		protected TSummaryItem SummaryItem
		{
			get
			{
				IWorkflowItemToolContext<TSummaryItem> context = (IWorkflowItemToolContext<TSummaryItem>)this.ContextBase;
				return CollectionUtils.FirstElement(context.SelectedItems);
			}
		}

		public void Open()
		{
			IWorkflowItemToolContext context = (IWorkflowItemToolContext) this.ContextBase;
			Open(this.OrderRef, this.Title, context.DesktopWindow);
		}

		private static void Open(EntityRef orderRef, string title, IDesktopWindow desktopWindow)
		{
			Platform.CheckForNullReference(orderRef, "orderRef");

			try
			{
				ApplicationComponent.LaunchAsDialog(desktopWindow,
					new PreliminaryDiagnosisConversationComponent(orderRef),
					title);
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, desktopWindow);
			}
		}
	}
}