using System;
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;

namespace ClearCanvas.Common.Application 
{
	/// <summary>
	/// Manages workspaces.
	/// </summary>
	/// <remarks>
	/// <b>WorkspaceManager</b> manages a collection of <see cref="Workspace"/> objects.
	/// It is accessed via the <see cref="ModelPlugin.WorkspaceManager"/> static property.  
	/// </remarks>
	public class WorkspaceManager
	{
		private WorkspaceCollection m_Workspaces = new WorkspaceCollection();
		private event EventHandler<WorkspaceEventArgs> m_WorkspaceActivatedEvent;
		private Workspace m_ActiveWorkspace;

		internal WorkspaceManager()
		{
			m_Workspaces.ItemAdded += new EventHandler<WorkspaceEventArgs>(OnWorkspaceAdded);
			m_Workspaces.ItemRemoved += new EventHandler<WorkspaceEventArgs>(OnWorkspaceRemoved);
		}

		public WorkspaceCollection Workspaces
		{
			get { return m_Workspaces; }
		}

		/// <summary>
		/// Gets or sets the currently active <see cref="Workspace"/>.
		/// </summary>
		/// <remarks>
		/// Cannot be set to <b>null</b>; there must always be an active workspace. By default,
		/// when a new workspace is added, that workspace is set as active.
		/// </remarks>
		/// <value>The currently active <see cref="Workspace"/> or <b>null</b> if
		/// there are no workspaces in the <see cref="WorkspaceManager"/>.</value>
		/// <exception cref="ArgumentNullException"><paramref name="ActiveWorkspace"/> is set to <b>null</b>.</exception>
		public Workspace ActiveWorkspace
		{
			get
			{
				if (this.Workspaces.Count == 0)
					return null;

				Platform.CheckMemberIsSet(m_ActiveWorkspace, "ActiveWorkspace");

				return m_ActiveWorkspace;
			}
			private set
			{
				Platform.CheckForNullReference(value, "ActiveWorkspace");

				// Don't bother if nothing's changed
				if (m_ActiveWorkspace == value)
					return;

				// Set the existing active workspace to inactive
				if (m_ActiveWorkspace != null)
					m_ActiveWorkspace.IsActivated = false;

				// Set the new active workspace
				m_ActiveWorkspace = value;

				// Let everyone know there's a new active workspace
				EventsHelper.Fire(m_WorkspaceActivatedEvent, this, new WorkspaceEventArgs(m_ActiveWorkspace));
			}
		}

		/// <summary>
		/// Occurs when a <see cref="Workspace"/> is activated using the <see cref="ActiveWorkspace"/> property.
		/// </summary>
		/// <remarks>The event handler receives an argument of type <see cref="WorkspaceEventArgs"/>.</remarks>
		public event EventHandler<WorkspaceEventArgs> WorkspaceActivated
		{
			add { m_WorkspaceActivatedEvent += value; }
			remove { m_WorkspaceActivatedEvent -= value; }
		}

		private void OnWorkspaceAdded(object sender, WorkspaceEventArgs e)
		{
			e.Workspace.ActivationChangedEvent += new EventHandler<ActivationChangedEventArgs>(OnActivationChanged);
			// Set the workspace just added to active
			e.Workspace.IsActivated = true;

            // NB- this is now done internally when the workspace is activated
            //e.Workspace.InitializeTools();
		}

		private void OnWorkspaceRemoved(object sender, WorkspaceEventArgs e)
		{
			e.Workspace.ActivationChangedEvent -= new EventHandler<ActivationChangedEventArgs>(OnActivationChanged);
			e.Workspace.Cleanup();

			// Make sure that we remove the reference to the last active workspace so
			// it can be swept up by the garbage collector			
			if (this.Workspaces.Count == 0)
				m_ActiveWorkspace = null;
		}

		private void OnActivationChanged(object sender, ActivationChangedEventArgs e)
		{
			this.ActiveWorkspace = sender as Workspace;
		}
	}
}