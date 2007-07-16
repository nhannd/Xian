using System;
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Represents the collection of <see cref="Workspace"/> objects for a given desktop window.
    /// </summary>
    public class WorkspaceCollection : DesktopObjectCollection<Workspace>
	{
        private DesktopWindow _owner;
        private Workspace _activeWorkspace;

        protected internal WorkspaceCollection(DesktopWindow owner)
		{
            _owner = owner;
		}

        /// <summary>
        /// Opens a new workspace.
        /// </summary>
        /// <param name="component"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public Workspace AddNew(IApplicationComponent component, string title)
        {
            return AddNew(component, title, null);
        }

        /// <summary>
        /// Opens a new workspace.
        /// </summary>
        /// <param name="component"></param>
        /// <param name="title"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public Workspace AddNew(IApplicationComponent component, string title, string name)
        {
            return AddNew(new WorkspaceCreationArgs(component, title, name));
        }

        /// <summary>
        /// Opens a new workspace.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public Workspace AddNew(WorkspaceCreationArgs args)
        {
            Workspace workspace = CreateWorkspace(args);
            Open(workspace);
            return workspace;
        }

        /// <summary>
        /// Gets the currently active workspace, or null if there are no workspaces in the collection.
        /// </summary>
        public Workspace ActiveWorkspace
        {
            get { return _activeWorkspace; }
        }

        protected virtual Workspace CreateWorkspace(WorkspaceCreationArgs args)
        {
            IWorkspaceFactory factory = CollectionUtils.FirstElement<IWorkspaceFactory>(
                (new WorkspaceFactoryExtensionPoint()).CreateExtensions()) ?? new DefaultWorkspaceFactory();

            return factory.CreateWorkspace(args, _owner);
        }

        protected override void OnItemActivationChangedInternal(ItemEventArgs<Workspace> args)
        {
            if (args.Item.Active)
            {
                // activated
                Workspace lastActive = _activeWorkspace;

                // set this prior to firing any events, so that a call to ActiveWorkspace property will return correct value
                _activeWorkspace = args.Item;

                if (lastActive != null)
                {
                    lastActive.RaiseActiveChanged();
                }
                _activeWorkspace.RaiseActiveChanged();
                
            }
        }

        protected override void OnItemClosed(ClosedItemEventArgs<Workspace> args)
        {
            if (this.Count == 0)
            {
                // raise pending de-activation event for the last active workspace, before the closing event
                if (_activeWorkspace != null)
                {
                    Workspace lastActive = _activeWorkspace;

                    // set this prior to firing any events, so that a call to ActiveWorkspace property will return correct value
                    _activeWorkspace = null;
                    lastActive.RaiseActiveChanged();
                }
            }

            base.OnItemClosed(args);
        }
    }
}
