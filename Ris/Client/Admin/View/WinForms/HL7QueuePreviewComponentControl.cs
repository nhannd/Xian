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

using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.Common;

namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    public partial class HL7QueuePreviewComponentControl : UserControl
    {
        private readonly HL7QueuePreviewComponent _component;

        public HL7QueuePreviewComponentControl(HL7QueuePreviewComponent component)
        {
            InitializeComponent();

            _component = component;

            _queue.Table = _component.Queue;
            _queue.MenuModel = _component.MenuModel;
            _queue.ToolbarModel = _component.ToolbarModel;
            
            _message.DataBindings.Add("Text", _component, "Message", true, DataSourceUpdateMode.OnPropertyChanged);

            _direction.DataSource = _component.DirectionChoices;
            _direction.DataBindings.Add("Value", _component, "Direction", true, DataSourceUpdateMode.OnPropertyChanged);
            _directionChkBox.DataBindings.Add("Checked", _component, "DirectionChecked", true, DataSourceUpdateMode.OnPropertyChanged);

			// Peer filter disabled permanently until proven usable again
			//_peer.DataSource = _component.PeerChoices;
			//_peer.DataBindings.Add("Value", _component, "Peer", true, DataSourceUpdateMode.OnPropertyChanged);
			//_peerChkBox.DataBindings.Add("Checked", _component, "PeerChecked", true, DataSourceUpdateMode.OnPropertyChanged);

            _type.DataSource = _component.TypeChoices;
            _type.DataBindings.Add("Value", _component, "Type", true, DataSourceUpdateMode.OnPropertyChanged);
            _typeChkBox.DataBindings.Add("Checked", _component, "TypeChecked", true, DataSourceUpdateMode.OnPropertyChanged);

            _status.DataSource = _component.StatusChoices;
            _status.DataBindings.Add("Value", _component, "Status", true, DataSourceUpdateMode.OnPropertyChanged);
            _statusChkBox.DataBindings.Add("Checked", _component, "StatusChecked", true, DataSourceUpdateMode.OnPropertyChanged);

            _createdOnStart.DataBindings.Add("Value", _component, "CreatedOnStart", true, DataSourceUpdateMode.OnPropertyChanged);
            _createdOnEnd.DataBindings.Add("Value", _component, "CreatedOnEnd", true, DataSourceUpdateMode.OnPropertyChanged);
            _updatedOnStart.DataBindings.Add("Value", _component, "UpdatedOnStart", true, DataSourceUpdateMode.OnPropertyChanged);
            _updatedOnEnd.DataBindings.Add("Value", _component, "UpdatedOnEnd", true, DataSourceUpdateMode.OnPropertyChanged);

            _createdOnStart.Value = Platform.Time.Date;
            _createdOnEnd.Value = Platform.Time.Date.AddDays(1);
            _updatedOnStart.Value = Platform.Time.Date;
            _updatedOnEnd.Value = Platform.Time.Date.AddDays(1);
        }

        private void _queue_SelectionChanged(object sender, EventArgs e)
        {
            _component.SetSelectedItem(_queue.Selection);
        }

        private void _showAll_Click(object sender, EventArgs e)
        {
            using (new CursorManager(this, Cursors.WaitCursor))
            {
                _component.ShowAllItems();
            }
        }

        private void _searchButton_Click(object sender, EventArgs e)
        {
            using (new CursorManager(this, Cursors.WaitCursor))
            {
                _component.ShowFilteredItems();
            }
        }

        private void _directionChkBox_CheckedChanged(object sender, EventArgs e)
        {
            _direction.Enabled = _directionChkBox.Checked;
        }

		// Peer filter disabled permanently until proven usable again
		//private void _peerChkBox_CheckedChanged(object sender, EventArgs e)
		//{
		//    _peer.Enabled = _peerChkBox.Checked;
		//}

        private void _typeChkBox_CheckedChanged(object sender, EventArgs e)
        {
            _type.Enabled = _typeChkBox.Checked;
        }

        private void _statusChkBox_CheckedChanged(object sender, EventArgs e)
        {
            _status.Enabled = _statusChkBox.Checked;
        }
    }
}
