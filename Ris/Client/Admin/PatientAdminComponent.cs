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
using ClearCanvas.Ris.Services;

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
        PatientProfile SelectedPatient { get; }
        event EventHandler SelectedPatientChanged;

        ClickHandlerDelegate DefaultActionHandler { get; set; }

        IDesktopWindow DesktopWindow { get; }
    }

    [AssociateView(typeof(PatientAdminComponentViewExtensionPoint))]
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

            public PatientProfile SelectedPatient
            {
                get { return _component._selectedPatient; }
            }

            public ClickHandlerDelegate DefaultActionHandler
            {
                get { return _component._defaultActionHandler; }
                set { _component._defaultActionHandler = value; }
            }

            public IDesktopWindow DesktopWindow
            {
                get { return _component.Host.DesktopWindow; }
            }
        }
        
        private event EventHandler _workingSetChanged;
        private TableData<PatientProfile> _workingSet;
        private PatientProfile _selectedPatient;
        private event EventHandler _selectedPatientChanged;
        private ClickHandlerDelegate _defaultActionHandler;
        private IPatientAdminService _patientAdminService;
        private ToolSet _toolSet;

        public PatientAdminComponent()
        {
        }

        public override void Start()
        {
            base.Start();
            _patientAdminService = ApplicationContext.GetService<IPatientAdminService>();
            _patientAdminService.PatientProfileChanged += _patientAdminService_PatientChanged;

            _workingSet = new TableData<PatientProfile>();
            _workingSet.Columns.Add(new TableColumn<PatientProfile, string>("MRN", delegate(PatientProfile p) { return p.MRN != null ? p.MRN.Id : ""; }));
            _workingSet.Columns.Add(new TableColumn<PatientProfile, string>("Name", delegate(PatientProfile p) { return p.Name.Format(); }));
            _workingSet.Columns.Add(new TableColumn<PatientProfile, string>("Sex", delegate(PatientProfile p) { return _patientAdminService.SexEnumTable[p.Sex].Value; }));
            _workingSet.Columns.Add(new TableColumn<PatientProfile, string>("Date of Birth", delegate(PatientProfile p) { return p.DateOfBirth.Date.ToShortDateString(); }));

            _toolSet = new ToolSet(new PatientAdminToolExtensionPoint(), new PatientAdminToolContext(this));
        }

        public override void Stop()
        {
            _patientAdminService.PatientProfileChanged -= _patientAdminService_PatientChanged;

            base.Stop();
        }

        private void _patientAdminService_PatientChanged(object sender, EntityChangeEventArgs e)
        {
            long oid = e.Change.EntityOID;

            // check if the patient with this oid is in the list
            int index = _workingSet.FindIndex(delegate(PatientProfile p) { return p.OID == oid; });
            if (index > -1)
            {
                PatientProfile p = _patientAdminService.LoadPatient(oid);
                // update the patient in the list
                _workingSet[index] = p;
            }
        }

        public void SetSearchCriteria(PatientProfileSearchCriteria criteria)
        {
            /* create some fake data
            List<PatientProfile> data = new List<PatientProfile>();

            PatientProfile p1 = PatientProfile.New();
            p1.Name.FamilyName = "Bean";
            p1.Name.GivenName = "Jim";
            p1.PatientId = "1122";
            data.Add(p1);

            PatientProfile p2 = PatientProfile.New();
            p2.Name.FamilyName = "Jones";
            p2.Name.GivenName = "Sally";
            p2.PatientId = "3344";
            data.Add(p2);


            _workingSetTableData.Fill(data);
             * */

            // obtain a list of patients matching the specified criteria
            IList<PatientProfile> patients = _patientAdminService.ListPatients(criteria);
            _workingSet.Clear();
            foreach (PatientProfile patient in patients)
                _workingSet.Add(patient);

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
            _selectedPatient = (PatientProfile)selection.Item;
            EventsHelper.Fire(_selectedPatientChanged, this, new EventArgs());
        }
    }
}
