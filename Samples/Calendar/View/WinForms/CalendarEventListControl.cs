using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ClearCanvas.Ris.Client.Calendar.View.WinForms
{
    public partial class CalendarEventListControl : UserControl
    {
        private CalendarEventListComponent _component;

        public CalendarEventListControl(CalendarEventListComponent component)
        {
            InitializeComponent();

            _component = component;
            _eventsTable.DataSource = _component.EventData;
        }
    }
}
