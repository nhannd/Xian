using System;
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// Summary description for WorkspaceCollection.
	/// </summary>
	public class WorkspaceCollection : ObservableList<Workspace, WorkspaceEventArgs>
	{
		public WorkspaceCollection()
		{
		}
		
        // This method is overridden here to force the Mono compiler to see it as "public"
        // Otherwise, a bug in the Mono compiler prevents the method from being visible 
        // to other assemblies
		new public event EventHandler<WorkspaceEventArgs> ItemAdded
		{
			add { base.ItemAdded += value; }
			remove { base.ItemAdded -= value;	}
		}

        // This method is overridden here to force the Mono compiler to see it as "public"
        // Otherwise, a bug in the Mono compiler prevents the method from being visible 
        // to other assemblies
        new public event EventHandler<WorkspaceEventArgs> ItemRemoved
		{
			add { base.ItemRemoved += value; }
			remove { base.ItemRemoved -= value; }
		}
	}
}
