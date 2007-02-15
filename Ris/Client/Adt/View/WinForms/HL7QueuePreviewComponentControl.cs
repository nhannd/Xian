using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Controls.WinForms;
using ClearCanvas.Common;

namespace ClearCanvas.Ris.Client.Adt.View.WinForms
{
    public partial class HL7QueuePreviewComponentControl : UserControl
    {
        private HL7QueuePreviewComponent _component;

        public HL7QueuePreviewComponentControl(HL7QueuePreviewComponent component)
        {
            InitializeComponent();

            if (this.DesignMode)
                return;

            _component = component;

            _queue.Table = _component.Queue;
            _queue.MenuModel = _component.MenuModel;
            _queue.ToolbarModel = _component.ToolbarModel;
            
            _message.DataBindings.Add("Text", _component, "Message", true, DataSourceUpdateMode.OnPropertyChanged);

            _direction.DataSource = _component.DirectionChoices;
            _direction.DataBindings.Add("Value", _component, "Direction", true, DataSourceUpdateMode.OnPropertyChanged);
            _directionChkBox.DataBindings.Add("Checked", _component, "DirectionChecked", true, DataSourceUpdateMode.OnPropertyChanged);

            _peer.DataSource = _component.PeerChoices;
            _peer.DataBindings.Add("Value", _component, "Peer", true, DataSourceUpdateMode.OnPropertyChanged);
            _peerChkBox.DataBindings.Add("Checked", _component, "PeerChecked", true, DataSourceUpdateMode.OnPropertyChanged);

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

        private void _peerChkBox_CheckedChanged(object sender, EventArgs e)
        {
            _peer.Enabled = _peerChkBox.Checked;
        }

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
