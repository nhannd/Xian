using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

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
        }

        private void _refresh_Click(object sender, EventArgs e)
        {
            _component.RefreshItems();
        }
    }
}
