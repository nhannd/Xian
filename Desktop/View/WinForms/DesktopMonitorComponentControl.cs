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
using System.Windows.Forms;

namespace ClearCanvas.Desktop.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="DesktopMonitorComponent"/>
    /// </summary>
    public partial class DesktopMonitorComponentControl : ApplicationComponentUserControl
    {
        private DesktopMonitorComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public DesktopMonitorComponentControl(DesktopMonitorComponent component)
            :base(component)
        {
            InitializeComponent();

            _component = component;

            _windows.Table = _component.Windows;
            _windows.DataBindings.Add("Selection", _component, "SelectedWindow", true, DataSourceUpdateMode.OnPropertyChanged);

            _workspaces.Table = _component.Workspaces;
            _workspaces.DataBindings.Add("Selection", _component, "SelectedWorkspace", true, DataSourceUpdateMode.OnPropertyChanged);
            _shelves.Table = _component.Shelves;
            _shelves.DataBindings.Add("Selection", _component, "SelectedShelf", true, DataSourceUpdateMode.OnPropertyChanged);

            _events.Table = _component.EventLog;
        }

        private void _openWindow_Click(object sender, EventArgs e)
        {
            _component.OpenWindow();
        }

        private void _activateWindow_Click(object sender, EventArgs e)
        {
            _component.ActivateSelectedWindow();
        }

        private void _closeWindow_Click(object sender, EventArgs e)
        {
            _component.CloseSelectedWindow();
        }

        private void _openWorkspace_Click(object sender, EventArgs e)
        {
            _component.OpenWorkspace();
        }

        private void _activateWorkspace_Click(object sender, EventArgs e)
        {
            _component.ActivateSelectedWorkspace();
        }

        private void _closeWorkspace_Click(object sender, EventArgs e)
        {
            _component.CloseSelectedWorkspace();
        }

        private void _openShelf_Click(object sender, EventArgs e)
        {
            _component.OpenShelf();
        }

        private void _activateShelf_Click(object sender, EventArgs e)
        {
            _component.ActivateSelectedShelf();
        }

        private void _showShelf_Click(object sender, EventArgs e)
        {
            _component.ShowSelectedShelf();
        }

        private void _hideShelf_Click(object sender, EventArgs e)
        {
            _component.HideSelectedShelf();
        }

        private void _closeShelf_Click(object sender, EventArgs e)
        {
            _component.CloseSelectedShelf();
        }
    }
}
