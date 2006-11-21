using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Controls.WinForms;

namespace ClearCanvas.Ris.Client.Adt.View.WinForms
{
    public partial class HL7QueuePreviewComponentControl : UserControl
    {
        private HL7QueuePreviewComponent _component;

        public HL7QueuePreviewComponentControl(HL7QueuePreviewComponent component)
        {
            InitializeComponent();

            _component = component;

            _queue.Table = _component.Queue;

            _message.DataBindings.Add("Text", _component, "Message", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void _queue_SelectionChanged(object sender, EventArgs e)
        {
            _component.SetSelectedItem(_queue.CurrentSelection);
        }

        private void _process_Click(object sender, EventArgs e)
        {
            using (new CursorManager(this, Cursors.WaitCursor))
            {
                _component.ProcessSelection();
            }
        }

        private void _showAll_Click(object sender, EventArgs e)
        {
            _component.ShowAllItems();
        }

        private void _showPending_Click(object sender, EventArgs e)
        {
            _component.ShowNextPendingBatchItems();
        }

        private void _resync_Click(object sender, EventArgs e)
        {
            using (new CursorManager(this, Cursors.WaitCursor))
            {
                _component.SyncQueues();
            }
        }
    }
}
