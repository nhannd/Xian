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

using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Represents the collection of <see cref="Workspace"/> objects for a given desktop window.
    /// </summary>
    public sealed class WorkspaceCollection : DesktopObjectCollection<Workspace>
	{
        private DesktopWindow _owner;
        private Workspace _activeWorkspace;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="owner"></param>
        internal WorkspaceCollection(DesktopWindow owner)
		{
            _owner = owner;
        }

        #region Public properties

        /// <summary>
        /// Gets the currently active workspace, or null if there are no workspaces in the collection.
        /// </summary>
        public Workspace ActiveWorkspace
        {
            get { return _activeWorkspace; }
        }

        #endregion

        #region Public methods

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

        #endregion

        #region Protected overridables

		/// <summary>
		/// Called when a <see cref="Workspace"/> item's <see cref="DesktopObject.InternalActiveChanged"/> event
		/// has fired.
		/// </summary>
        protected sealed override void OnItemActivationChangedInternal(ItemEventArgs<Workspace> args)
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

		/// <summary>
		/// Called when a <see cref="Workspace"/> item's <see cref="DesktopObject.Closed"/> event
		/// has fired.
		/// </summary>
		protected sealed override void OnItemClosed(ClosedItemEventArgs<Workspace> args)
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

        #endregion

        #region Helpers

        private Workspace CreateWorkspace(WorkspaceCreationArgs args)
        {
            IWorkspaceFactory factory = CollectionUtils.FirstElement<IWorkspaceFactory>(
                (new WorkspaceFactoryExtensionPoint()).CreateExtensions()) ?? new DefaultWorkspaceFactory();

            return factory.CreateWorkspace(args, _owner);
        }

        #endregion
    }
}
