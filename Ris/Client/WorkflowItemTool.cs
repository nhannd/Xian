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
					// This invalidates the selected folder so that any updates effected by the tools Execute method are reflected.
					// Additionally, if the Execute method opens an ApplicationComponent, updates effected by the component's Start method
					// are reflected.  Changes effected by an ApplicationComponent after the Start method are not reflected in the invalidated
					// folder.  In this case, the selected folder should be invalidated again when the component closes.
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
