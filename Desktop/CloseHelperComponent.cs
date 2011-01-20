#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Tables;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Extension point for views onto <see cref="CloseHelperComponent"/>.
    /// </summary>
    [ExtensionPoint]
	public sealed class CloseHelperComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// The <see cref="CloseHelperComponent"/> assists the user in completing remaining work 
    /// that is preventing open workspaces from closing.
    /// </summary>
    /// <remarks>
    /// This component is shown when an <see cref="IDesktopWindow"/> is being closed or 
    /// the application is shutting down.
    /// </remarks>
    [AssociateView(typeof(CloseHelperComponentViewExtensionPoint))]
    public class CloseHelperComponent : ApplicationComponent
    {
        private Table<Workspace> _workspaces;
        private Workspace _selectedWorkspace;

        /// <summary>
        /// Constructor.
        /// </summary>
        public CloseHelperComponent()
        {
            _workspaces = new Table<Workspace>();
            _workspaces.Columns.Add(new TableColumn<Workspace, string>("Workspace", delegate(Workspace w) { return w.Title; }));
        }

		/// <summary>
		/// Refreshes the active <see cref="IWorkspace"/>s and determines which ones cannot be
		/// closed because there is input required from the user.
		/// </summary>
        public void Refresh()
        {
            UnsubscribeWorkspaces(_workspaces.Items);
            _workspaces.Items.Clear();

            ProcessWindow(this.Host.DesktopWindow);
        }

        private void ProcessWindow(DesktopWindow window)
        {
            ICollection<Workspace> unreadyWorkspaces = CollectionUtils.Select<Workspace>(window.Workspaces,
                delegate(Workspace w) { return !w.QueryCloseReady(); });
            SubscribeWorkspaces(unreadyWorkspaces);
            _workspaces.Items.AddRange(unreadyWorkspaces);
        }

        private void SubscribeWorkspaces(IEnumerable<Workspace> workspaces)
        {
            foreach (Workspace w in workspaces)
                w.Closed += WorkspaceClosedEventHandler;
        }

        private void UnsubscribeWorkspaces(IEnumerable<Workspace> workspaces)
        {
            foreach (Workspace w in workspaces)
                w.Closed -= WorkspaceClosedEventHandler;
        }

        private void WorkspaceClosedEventHandler(object sender, ClosedEventArgs e)
        {
            _workspaces.Items.Remove((Workspace)sender);
        }

    	/// <summary>
    	/// Called by the host to initialize the application component.
    	/// </summary>
    	///  <remarks>
    	/// Override this method to implement custom initialization logic.  Overrides must be sure to call the base implementation.
    	/// </remarks>
    	public override void Start()
        {
            base.Start();
        }

    	/// <summary>
    	/// Called by the host when the application component is being terminated.
    	/// </summary>
    	/// <remarks>
    	/// Override this method to implement custom termination logic.  Overrides must be sure to call the base implementation.
    	/// </remarks>
    	public override void Stop()
        {
            UnsubscribeWorkspaces(_workspaces.Items);
            base.Stop();
        }

		/// <summary>
		/// Gets an <see cref="ITable"/> containing the workspaces that still require user input in order to close.
		/// </summary>
        public ITable Workspaces
        {
            get { return _workspaces; }
        }

		/// <summary>
		/// Gets or sets the currently selected <see cref="IWorkspace"/>.
		/// </summary>
		/// <remarks>
		/// When the selected workspace is set, it is immediately activated.
		/// </remarks>
        public ISelection SelectedWorkspace
        {
            get { return new Selection(_selectedWorkspace); }
            set
            {
                _selectedWorkspace = (Workspace)value.Item;
                if (_selectedWorkspace != null)
                {
                    _selectedWorkspace.Activate();
                }
            }
        }
    }
}
