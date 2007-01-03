using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Healthcare;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Services;
using ClearCanvas.Enterprise;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Ris.Client.Common;

namespace ClearCanvas.Ris.Client.Adt
{
    /// <summary>
    /// Extension point for views onto <see cref="VisitPractitionerSummaryComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class VisitPractitionerSummaryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// VisitPractitionerSummaryComponent class
    /// </summary>
    [AssociateView(typeof(VisitPractitionerSummaryComponentViewExtensionPoint))]
    public class VisitPractitionersSummaryComponent : ApplicationComponent
    {
        private Visit _visit;
        private VisitPractitioner _currentVisitPractitionerSelection;
        private Table<VisitPractitioner> _practitionersTable;

        private IAdtService _adtService;
        private IPractitionerService _practitionerService;

        private VisitPractitionerRoleEnumTable _visitPractitionerRoleTable;

        private CrudActionModel _visitPractitionerActionHandler;

        /// <summary>
        /// Constructor
        /// </summary>
        public VisitPractitionersSummaryComponent()
        {
            _adtService = ApplicationContext.GetService<IAdtService>();
            _practitionerService = ApplicationContext.GetService<IPractitionerService>();

            _visitPractitionerRoleTable = _adtService.GetVisitPractitionerRoleEnumTable();

            _practitionersTable = new Table<VisitPractitioner>();

            _practitionersTable.Columns.Add(new TableColumn<VisitPractitioner, string>(
                SR.ColumnRole,
                delegate(VisitPractitioner vp)
                {
                    return _visitPractitionerRoleTable[vp.Role].Value;
                },
                0.8f));
            _practitionersTable.Columns.Add(new TableColumn<VisitPractitioner, string>(
                SR.ColumnPractitioner,
                delegate(VisitPractitioner vp)
                {
                    return vp.Practitioner.Format();
                },
                2.5f));
            _practitionersTable.Columns.Add(new TableColumn<VisitPractitioner, string>(
                SR.ColumnStartTime,
                delegate(VisitPractitioner vp)
                {
                    return Format.DateTime(vp.StartTime);
                },
                0.8f));
            _practitionersTable.Columns.Add(new TableColumn<VisitPractitioner, string>(
                SR.ColumnEndTime,
                delegate(VisitPractitioner vp)
                {
                    return Format.DateTime(vp.EndTime);
                },
                0.8f));

            _visitPractitionerActionHandler = new CrudActionModel();
            _visitPractitionerActionHandler.Add.SetClickHandler(AddVisitPractitioner);
            _visitPractitionerActionHandler.Edit.SetClickHandler(UpdateSelectedVisitPractitioner);
            _visitPractitionerActionHandler.Delete.SetClickHandler(DeleteSelectedVisitPractitioner);

            _visitPractitionerActionHandler.Add.Enabled = true;
        }

        public override void Start()
        {
            // TODO prepare the component for its live phase
            base.Start();
        }

        public override void Stop()
        {
            // TODO prepare the component to exit the live phase
            // This is a good place to do any clean up
            base.Stop();
        }

        public Visit Visit
        {
            get { return _visit; }
            set { _visit = value; }
        }

        public ITable VisitPractitioners
        {
            get { return _practitionersTable; }
        }

        public ActionModelNode VisitPractionersActionModel
        {
            get { return _visitPractitionerActionHandler; }
        }

        public VisitPractitioner CurrentVisitPractitionerSelection
        {
            get { return _currentVisitPractitionerSelection; }
            set
            {
                _currentVisitPractitionerSelection = value;
                VisitPractitionerSelectionChanged();
            }
        }

        public void SetSelectedVisitPractitioner(ISelection selection)
        {
            this.CurrentVisitPractitionerSelection = (VisitPractitioner)selection.Item;
        }

        private void VisitPractitionerSelectionChanged()
        {
            if (_currentVisitPractitionerSelection != null)
            {
                _visitPractitionerActionHandler.Edit.Enabled = true;
                _visitPractitionerActionHandler.Delete.Enabled = true;
            }
            else
            {
                _visitPractitionerActionHandler.Edit.Enabled = false;
                _visitPractitionerActionHandler.Delete.Enabled = false;
            }
        }

        public void AddVisitPractitioner()
        {
            DummyAddVisitPractitioner();

            LoadVisitPractioners();
            this.Modified = true;
        }

        public void UpdateSelectedVisitPractitioner()
        {
            DummyUpdateVisitPractitioner();

            LoadVisitPractioners();
            this.Modified = true;
        }

        public void DeleteSelectedVisitPractitioner()
        {
            _visit.Practitioners.Remove(_currentVisitPractitionerSelection);

            LoadVisitPractioners();
            this.Modified = true;
        }

        public void LoadVisitPractioners()
        {
            _practitionersTable.Items.Clear();
            _practitionersTable.Items.AddRange(_visit.Practitioners);
        }

        #region Dummy Code

        private void DummyAddVisitPractitioner()
        {
            IList<Practitioner> practitioners = _practitionerService.FindPractitioners("Who", null);
            if (practitioners.Count == 0)
            {
                Practitioner p = new Practitioner();
                p.LicenseNumber = "123456";
                p.Name.FamilyName = "Who";
                p.Name.GivenName = "Doctor";

                _practitionerService.AddPractitioner(p);

                practitioners = _practitionerService.FindPractitioners("Who", null);
            }

            VisitPractitioner vp = new VisitPractitioner();

            vp.Role = VisitPractitionerRole.RF;
            vp.Practitioner = practitioners[0];
            vp.StartTime = Platform.Time;

            _visit.Practitioners.Add(vp);
        }

        private void DummyUpdateVisitPractitioner()
        {
            VisitPractitioner vp = _currentVisitPractitionerSelection;
            vp.Role = VisitPractitionerRole.CN;
            vp.EndTime = Platform.Time;
        }
    	#endregion    
    }
}
