using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Adt.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="VisitSummaryComponent"/>
    /// </summary>
    public partial class VisitSummaryComponentControl : CustomUserControl
    {
        private VisitSummaryComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public VisitSummaryComponentControl(VisitSummaryComponent component)
        {
            InitializeComponent();

            _component = component;

            _visits.Table = _component.Visits;
            _visits.ToolbarModel = _component.VisitListActionModel;
            _visits.MenuModel = _component.VisitListActionModel;
        }

        private void VisitSummaryComponentControl_Load(object sender, EventArgs e)
        {
            _component.LoadVisitsTable();
        }

        private void _visits_SelectionChanged(object sender, EventArgs e)
        {
            _component.SetSelectedVisit(_visits.Selection);
        }

        private void _closeButton_Click(object sender, EventArgs e)
        {
            _component.Close();
        }
    }
}
