using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Abstract base class for tools that operate in a <see cref="IWorkflowItemToolContext{TItem}"/> based context.
	/// </summary>
	/// <typeparam name="TItem"></typeparam>
	/// <typeparam name="TContext"></typeparam>
    public abstract class WorkflowItemTool<TItem, TContext> : Tool<TContext>, IDropHandler<TItem>
        where TItem : WorklistItemSummaryBase
		where TContext : IWorkflowItemToolContext<TItem>
    {
        private readonly string _operationName;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="operationName">Specifies the name of the workflow operation that this tool invokes.</param>
        protected WorkflowItemTool(string operationName)
        {
            _operationName = operationName;
        }

		/// <summary>
		/// Gets a value indicating whether this tool is currently enabled.
		/// </summary>
		/// <remarks>
		/// The default implmentation is based on the workflow operation enablement. Subclasses may
		/// override this property to modify this behaviour.
		/// </remarks>
        public virtual bool Enabled
        {
            get
            {
                return this.Context.SelectedItems.Count == 1 &&
					this.Context.GetOperationEnablement(_operationName);
            }
        }

		/// <summary>
		/// Occurs when the value of <see cref="Enabled"/> changes.
		/// </summary>
        public event EventHandler EnabledChanged
        {
            add { this.Context.SelectionChanged += value; }
            remove { this.Context.SelectionChanged -= value; }
        }

		/// <summary>
		/// Invokes the workflow operation.
		/// </summary>
        public void Apply()
        {
            TItem item = CollectionUtils.FirstElement(this.Context.SelectedItems);
			try
			{
				bool success = Execute(item);
				if (success)
				{
					this.Context.InvalidateSelectedFolder();
				}
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Context.DesktopWindow);
			}
		}

		#region Protected API

		/// <summary>
		/// Gets the name of the workflow operation that this tool invokes.
		/// </summary>
        protected string OperationName
        {
            get { return _operationName; }
        }

		/// <summary>
		/// Called to invoke the workflow operation.
		/// </summary>
		/// <remarks>
		/// The method should return true if the operation was completed, in order to invalidate the currently selected folder.
		/// A return value of false implies that either the user cancelled, or the operation failed.  
		/// </remarks>
		/// <param name="item"></param>
		/// <returns></returns>
        protected abstract bool Execute(TItem item);

		#endregion

		#region IDropHandler<TItem> Members

		/// <summary>
		/// Asks the handler if it can accept the specified items.  This value is used to provide visual feedback
		/// to the user to indicate that a drop is possible.
		/// </summary>
		public virtual bool CanAcceptDrop(ICollection<TItem> items)
        {
            return this.Context.GetOperationEnablement(this.OperationName);
        }

		/// <summary>
		/// Asks the handler to process the specified items, and returns true if the items were successfully processed.
		/// </summary>
		public virtual bool ProcessDrop(ICollection<TItem> items)
        {
            TItem item = CollectionUtils.FirstElement(items);
			try
			{
				bool success = Execute(item);
				if (success)
				{
					this.Context.InvalidateSelectedFolder();
					return true;
				}
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Context.DesktopWindow);
			}
			return false;
		}

        #endregion
    }
}
