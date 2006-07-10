using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    [ExtensionOf(typeof(PatientAdminComponentViewExtensionPoint))]
    public class PatientAdminComponentView : WinFormsView, IApplicationComponentView
    {
        private PatientAdminControl _control;
        private PatientAdminComponent _component;

        public void SetComponent(IApplicationComponent component)
        {
            _component = (PatientAdminComponent)component;
            _component.WorkingSetChanged += new EventHandler(PatientAdminComponent_WorkingSetChanged);
        }

        public void PatientAdminComponent_WorkingSetChanged(object sender, EventArgs e)
        {
            this.Control.PatientTableView.RefreshRows();
        }

        protected PatientAdminControl Control
        {
            get
            {
                if (_control == null)
                {
                    _control = new PatientAdminControl();
                    _control.PatientTableView.SetTable(_component.SearchResults);
                    _control.PatientTableView.SelectionChanged += new EventHandler<TableViewEventArgs>(PatientTableView_SelectionChanged);
                    _control.PatientTableView.ItemDoubleClicked += new EventHandler<TableViewEventArgs>(PatientTableView_ItemDoubleClicked);
                }
                return _control;
            }
        }

        private void PatientTableView_ItemDoubleClicked(object sender, TableViewEventArgs e)
        {
            _component.OpenPatient(0);
        }

        private void PatientTableView_SelectionChanged(object sender, TableViewEventArgs e)
        {
        }

        public override object GuiElement
        {
            get
            {
                return this.Control;
            }
        }
    }
}
