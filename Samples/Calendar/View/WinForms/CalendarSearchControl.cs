using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ClearCanvas.Controls.WinForms;

namespace ClearCanvas.Ris.Client.Calendar.View.WinForms
{
    public partial class CalendarSearchControl : UserControl
    {
        private CalendarSearchComponent _component;

        public CalendarSearchControl(CalendarSearchComponent component)
        {
            InitializeComponent();

            _component = component;

            _fromDate.DataBindings.Add("Value", _component, "FromDate", true, DataSourceUpdateMode.OnPropertyChanged);
            _untilDate.DataBindings.Add("Value", _component, "UntilDate", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void _searchButton_Click(object sender, EventArgs e)
        {
            using (new CursorManager(this, Cursors.WaitCursor))
            {
                _component.Search();
            }
        }
    }
}
