#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
