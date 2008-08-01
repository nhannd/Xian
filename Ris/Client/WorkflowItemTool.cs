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

        protected WorkflowItemTool(string operationName)
        {
            _operationName = operationName;
        }

        public virtual bool Enabled
        {
            get
            {
                return this.Context.SelectedItems.Count == 1 &&
					this.Context.GetOperationEnablement(_operationName);
            }
        }

        public virtual event EventHandler EnabledChanged
        {
            add { this.Context.SelectionChanged += value; }
            remove { this.Context.SelectionChanged -= value; }
        }

        public virtual void Apply()
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

        protected string OperationName
        {
            get { return _operationName; }
        }

        protected abstract bool Execute(TItem item);

        #region IDropHandler<TItem> Members

        public virtual bool CanAcceptDrop(ICollection<TItem> items)
        {
            return this.Context.GetOperationEnablement(this.OperationName);
        }

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
