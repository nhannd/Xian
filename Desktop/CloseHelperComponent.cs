using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Extension point for views onto <see cref="CloseHelperComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class CloseHelperComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// CloseHelperComponent class
    /// </summary>
    [AssociateView(typeof(CloseHelperComponentViewExtensionPoint))]
    public class CloseHelperComponent : ApplicationComponent
    {
        private DecoratedTable<Workspace> _workspaces;
        private Workspace _selectedWorkspace;

        /// <summary>
        /// Constructor
        /// </summary>
        public CloseHelperComponent()
        {
            _workspaces = new DecoratedTable<Workspace>(2);
            _workspaces.Columns.Add(new DecoratedTableColumn<Workspace, string>("Workspace", delegate(Workspace w) { return w.Title; }, 1, 0));
            _workspaces.Columns.Add(new DecoratedTableColumn<Workspace, string>("Window", delegate(Workspace w) { return w.DesktopWindow.Title; }, 1, 1));
        }

        public void Refresh(bool thisWindowOnly)
        {
            UnsubscribeWorkspaces(_workspaces.Items);
            _workspaces.Items.Clear();

            if (thisWindowOnly)
            {
                ProcessWindow(this.Host.DesktopWindow);
            }
            else
            {
                foreach (DesktopWindow window in Application.DesktopWindows)
                {
                    ProcessWindow(window);
                }
            }
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

        public override void Start()
        {
            base.Start();
        }

        public override void Stop()
        {
            UnsubscribeWorkspaces(_workspaces.Items);
            base.Stop();
        }

        public ITable Workspaces
        {
            get { return _workspaces; }
        }

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
