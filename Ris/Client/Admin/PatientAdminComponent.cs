using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Presentation;

using ClearCanvas.Healthcare;

namespace ClearCanvas.Ris.Client.Admin
{
    [ExtensionPoint()]
    public class PatientAdminComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    [ExtensionPoint()]
    public class PatientAdminToolExtensionPoint : ExtensionPoint<ITool>
    {
    }

    public interface IPatientAdminToolContext : IToolContext
    {
        Patient SelectedPatient { get; }
        event EventHandler SelectedPatientChanged;

        ClickHandlerDelegate DefaultActionHandler { get; set; }
    }

    [ApplicationComponentView(typeof(PatientAdminComponentViewExtensionPoint))]
    public class PatientAdminComponent : ApplicationComponent
    {
        public class PatientAdminToolContext : ToolContext, IPatientAdminToolContext
        {
            private PatientAdminComponent _component;

            public PatientAdminToolContext(PatientAdminComponent component)
            {
                _component = component;
            }

            public event EventHandler SelectedPatientChanged
            {
                add { _component._selectedPatientChanged += value; }
                remove { _component._selectedPatientChanged -= value; }
            }

            public Patient SelectedPatient
            {
                get { return _component._selectedPatient; }
            }

            public ClickHandlerDelegate DefaultActionHandler
            {
                get { return _component._defaultActionHandler; }
                set { _component._defaultActionHandler = value; }
            }
        }
        
        private ToolSet _toolSet;
        private event EventHandler _workingSetChanged;
        private TableData<Patient> _workingSetTableData;
 
        private Patient _selectedPatient;
        private event EventHandler _selectedPatientChanged;

        private ClickHandlerDelegate _defaultActionHandler;

        public PatientAdminComponent()
        {
            TableColumn<Patient>[] columns = new TableColumn<Patient>[] {
                new TableColumn<Patient>("ID", delegate(Patient p) { return p.PatientId; }),
                new TableColumn<Patient>("Name", delegate(Patient p) { return p.PrimaryName; }),
            };

            _workingSetTableData = new TableData<Patient>(columns);

        }

        public override IToolSet ToolSet
        {
            get
            {
                if (_toolSet == null)
                {
                    _toolSet = new ToolSet(new PatientAdminToolExtensionPoint(), new PatientAdminToolContext(this));
                }
                return _toolSet;
            }
        }

        public void SetSearchCriteria(PatientSearchCriteria criteria)
        {
            // create some fake data
            List<Patient> data = new List<Patient>();

            Patient p1 = Patient.New();
            p1.PrimaryName = "Jim Bean";
            p1.PatientId = "1122";
            data.Add(p1);

            Patient p2 = Patient.New();
            p2.PrimaryName = "Sally Jones";
            p2.PatientId = "3344";
            data.Add(p2);


            _workingSetTableData.Fill(data);
            EventsHelper.Fire(_workingSetChanged, this, new EventArgs());
        }

        public event EventHandler WorkingSetChanged
        {
            add { _workingSetChanged += value; }
            remove { _workingSetChanged -= value; }
        }

        public ITableData WorkingSetTableData
        {
            get { return _workingSetTableData; }
        }

        public void RowDoubleClick()
        {
            if (_defaultActionHandler != null)
            {
                _defaultActionHandler();
            }
        }

        public void SetSelection(ISelection selection)
        {
            ITableRow row = selection.SelectedRow;
            _selectedPatient = (Patient)(row == null ? null : row.Item);
            EventsHelper.Fire(_selectedPatientChanged, this, new EventArgs());
        }
    }
}
