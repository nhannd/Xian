using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Enterprise;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

using ClearCanvas.Healthcare;
using System.ComponentModel;

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
        private TableData<PatientTableEntry> _workingSet;
 
        private Patient _selectedPatient;
        private event EventHandler _selectedPatientChanged;

        private ClickHandlerDelegate _defaultActionHandler;

        private IPatientAdminService _patientAdminService;

        public PatientAdminComponent()
        {
            _workingSet = new TableData<PatientTableEntry>();
        }

        public override void Start()
        {
            base.Start();
            _patientAdminService = ApplicationContext.GetService<IPatientAdminService>();
            _patientAdminService.PatientChanged += _patientAdminService_PatientChanged;
        }

        public override void Stop()
        {
            _patientAdminService.PatientChanged -= _patientAdminService_PatientChanged;

            base.Stop();
        }

        private void _patientAdminService_PatientChanged(object sender, EntityChangeEventArgs e)
        {
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
            /* create some fake data
            List<Patient> data = new List<Patient>();

            Patient p1 = Patient.New();
            p1.Name.FamilyName = "Bean";
            p1.Name.GivenName = "Jim";
            p1.PatientId = "1122";
            data.Add(p1);

            Patient p2 = Patient.New();
            p2.Name.FamilyName = "Jones";
            p2.Name.GivenName = "Sally";
            p2.PatientId = "3344";
            data.Add(p2);


            _workingSetTableData.Fill(data);
             * */

            // obtain a list of patients matching the specified criteria
            IList<Patient> patients = _patientAdminService.ListPatients(criteria);
            _workingSet.Clear();
            foreach (Patient patient in patients)
                _workingSet.Add(new PatientTableEntry(patient));

            EventsHelper.Fire(_workingSetChanged, this, new EventArgs());
        }

        public event EventHandler WorkingSetChanged
        {
            add { _workingSetChanged += value; }
            remove { _workingSetChanged -= value; }
        }

        public ITableData WorkingSet
        {
            get { return _workingSet; }
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
            PatientTableEntry entry = (PatientTableEntry)selection.Item;
            _selectedPatient = entry == null ? null : entry.Patient;
            EventsHelper.Fire(_selectedPatientChanged, this, new EventArgs());
        }
    }
}
