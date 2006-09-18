using System;
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// A collection of <see cref="IWorkspace"/> objects, used by <see cref="WorkspaceManager"/>
    /// </summary>
    public class WorkspaceCollection : ObservableList<IWorkspace, WorkspaceEventArgs>
	{
        private WorkspaceManager _owner;

		public WorkspaceCollection(WorkspaceManager owner)
		{
            _owner = owner;
		}
		
        // This method is overridden here to force the Mono compiler to see it as "public"
        // Otherwise, a bug in the Mono compiler prevents the method from being visible 
        // to other assemblies
		public override event EventHandler<WorkspaceEventArgs> ItemAdded
		{
			add { base.ItemAdded += value; }
			remove { base.ItemAdded -= value;	}
		}

        // This method is overridden here to force the Mono compiler to see it as "public"
        // Otherwise, a bug in the Mono compiler prevents the method from being visible 
        // to other assemblies
        public override event EventHandler<WorkspaceEventArgs> ItemRemoved
		{
			add { base.ItemRemoved += value; }
			remove { base.ItemRemoved -= value; }
		}

        /// <summary>
        /// Overridden to check if the workspace can be closed before removing it.  This method
        /// calls <see cref="IWorkspace.CanClose"/> to see if the workspace can be closed.
        /// </summary>
        /// <param name="item"></param>
        /// <returns>True if the workspace was successfully closed and removed, otherwise false</returns>
        public override bool Remove(IWorkspace item)
        {
            return item.CanClose() ? base.Remove(item) : false;
        }

        protected override void OnItemAdded(WorkspaceEventArgs e)
        {
            _owner.WorkspaceAdded(e.Workspace);
            base.OnItemAdded(e);
        }

        protected override void OnItemRemoved(WorkspaceEventArgs e)
        {
            _owner.WorkspaceRemoved(e.Workspace);
            base.OnItemRemoved(e);
        }
	}
}
