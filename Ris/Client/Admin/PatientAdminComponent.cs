using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
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

    public class PatientAdminToolContext : ToolContext
    {
        private PatientAdminComponent _component;

        public PatientAdminToolContext(PatientAdminComponent component)
            : base(new PatientAdminToolExtensionPoint())
        {
            _component = component;
        }

        public PatientAdminComponent Component
        {
            get { return _component; }
        }
    }

    public class PatientAdminComponent : ApplicationComponent
    {
        private ToolSet _toolSet;
        private event EventHandler _workingSetChanged;

        private TableData<Patient> _searchResults;

        public PatientAdminComponent()
        {
            TableColumn<Patient>[] columns = new TableColumn<Patient>[] {
                new TableColumn<Patient>("ID", delegate(Patient p) { return p.PatientId; }),
                new TableColumn<Patient>("Name", delegate(Patient p) { return p.PrimaryName; }),
            };

            _searchResults = new TableData<Patient>(columns);

        }

        public override IToolSet ToolSet
        {
            get
            {
                if (_toolSet == null)
                {
                    _toolSet = new ToolSet(new PatientAdminToolContext(this));
                }
                return _toolSet;
            }
        }

        public void SetCriteria()
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

            _searchResults.Fill(data);

            EventsHelper.Fire(_workingSetChanged, this, new EventArgs());
        }

        public event EventHandler WorkingSetChanged
        {
            add { _workingSetChanged += value; }
            remove { _workingSetChanged -= value; }
        }

        public ITableData SearchResults
        {
            get { return _searchResults; }
        }

        public void OpenPatient(int row)
        {
            Workspace workspace = new ApplicationComponentHostWorkspace(
                "Edit Patient",
                new PatientDetailComponent(),
                new PatientDetailComponentViewExtensionPoint());

            DesktopApplication.WorkspaceManager.Workspaces.Add(workspace);
        }
    }
}
