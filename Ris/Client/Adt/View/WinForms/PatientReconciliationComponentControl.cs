using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ClearCanvas.Controls.WinForms;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Adt.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="PatientReconciliationComponent"/>
    /// </summary>
    public partial class PatientReconciliationComponentControl : CustomUserControl
    {
        private PatientReconciliationComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public PatientReconciliationComponentControl(PatientReconciliationComponent component)
        {
            InitializeComponent();

            _component = component;

            _targetTableView.Table = _component.TargetProfiles;
            _reconciliationTableView.Table = _component.ReconciliationProfiles;

            Control targetPreview = (Control)_component.TargetPreviewComponentView.GuiElement;
            targetPreview.Dock = DockStyle.Fill;
            _leftPreviewPanel.Controls.Add(targetPreview);

            Control recPreview = (Control)_component.ReconciliationPreviewComponentView.GuiElement;
            recPreview.Dock = DockStyle.Fill;
            _rightPreviewPanel.Controls.Add(recPreview);
        }

        private void _reconcileButton_Click(object sender, EventArgs e)
        {
            using (new CursorManager(this, Cursors.WaitCursor))
            {
                _component.Reconcile();
            }
        }

        private void _cancelButton_Click(object sender, EventArgs e)
        {
            _component.Cancel();
        }


        private void _reconciliationTableView_SelectionChanged(object sender, EventArgs e)
        {
            using (new CursorManager(this, Cursors.WaitCursor))
            {
                _component.SetSelectedReconciliationProfile(_reconciliationTableView.CurrentSelection);
            }
        }

        private void _targetTableView_SelectionChanged(object sender, EventArgs e)
        {
            using (new CursorManager(this, Cursors.WaitCursor))
            {
                _component.SetSelectedTargetProfile(_targetTableView.CurrentSelection);
            }
        }

    }
}
