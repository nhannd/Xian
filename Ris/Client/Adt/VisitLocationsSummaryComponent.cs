using System;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Services;
using ClearCanvas.Enterprise;
using ClearCanvas.Desktop.Actions;
using System.Collections.Generic;

namespace ClearCanvas.Ris.Client.Adt
{
    /// <summary>
    /// Extension point for views onto <see cref="VisitLocationsSummaryComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class VisitLocationsSummaryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// VisitLocationsSummaryComponent class
    /// </summary>
    [AssociateView(typeof(VisitLocationsSummaryComponentViewExtensionPoint))]
    public class VisitLocationsSummaryComponent : ApplicationComponent
    {
        private Visit _visit;
        private Table<VisitLocation> _locationsTable;
        private VisitLocation _currentVisitLocationSelection;

        private IAdtService _adtService;
        private IFacilityAdminService _facilityAdminService;
        private ILocationAdminService _locationAdminService;
        private VisitLocationRoleEnumTable _visitLocationRole;

        private CrudActionModel _visitLocationActionHandler;

        /// <summary>
        /// Constructor
        /// </summary>
        public VisitLocationsSummaryComponent()
        {
            _adtService = ApplicationContext.GetService<IAdtService>();
            _facilityAdminService = ApplicationContext.GetService<IFacilityAdminService>();

            _visitLocationRole = _adtService.GetVisitLocationRoleEnumTable();

            _locationsTable = new Table<VisitLocation>();

            _locationsTable.Columns.Add(new TableColumn<VisitLocation, string>(
                SR.ColumnRole,
                delegate(VisitLocation vl)
                {
                    return _visitLocationRole[vl.Role].Value;
                },
                0.8f));
            _locationsTable.Columns.Add(new TableColumn<VisitLocation, string>(
                SR.ColumnLocation,
                delegate(VisitLocation vl)
                {
                    return vl.Location.ToString();
                },
                2.5f));
            _locationsTable.Columns.Add(new TableColumn<VisitLocation, string>(
                SR.ColumnStartTime,
                delegate(VisitLocation vl)
                {
                    return Format.DateTime(vl.StartTime);
                },
                0.8f));
            _locationsTable.Columns.Add(new TableColumn<VisitLocation, string>(
                SR.ColumnEndTime,
                delegate(VisitLocation vl)
                {
                    return Format.DateTime(vl.EndTime);
                },
                0.8f));

            _visitLocationActionHandler = new CrudActionModel();
            _visitLocationActionHandler.Add.SetClickHandler(AddVisitLocation);
            _visitLocationActionHandler.Edit.SetClickHandler(UpdateSelectedVisitLocation);
            _visitLocationActionHandler.Delete.SetClickHandler(DeleteSelectedVisitLocation);

            _visitLocationActionHandler.Add.Enabled = true;
        }

        public override void Start()
        {
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

        public ITable Locations
        {
            get { return _locationsTable; }
        }

        public VisitLocation CurrentVisitLocationSelection
        {
            get { return _currentVisitLocationSelection; }
            set
            {
                _currentVisitLocationSelection = value;
                VisitLocationSelectionChanged();
            }
        }

        public void SetSelectedVisitLocation(ISelection selection)
        {
            this.CurrentVisitLocationSelection = (VisitLocation)selection.Item;
        }

        private void VisitLocationSelectionChanged()
        {
            if (_currentVisitLocationSelection != null)
            {
                _visitLocationActionHandler.Edit.Enabled = true;
                _visitLocationActionHandler.Delete.Enabled = true;
            }
            else
            {
                _visitLocationActionHandler.Edit.Enabled = false;
                _visitLocationActionHandler.Delete.Enabled = false;
            }
        }


        public ActionModelNode VisitLocationActionModel
        {
            get { return _visitLocationActionHandler; }
        }

        public void AddVisitLocation()
        {
            StubAddVisitLocation();
            
            LoadLocations();
            this.Modified = true;
        }

        private void StubAddVisitLocation()
        {
            IList<Facility> facilities = _facilityAdminService.GetAllFacilities();
            if (facilities.Count == 0)
            {
                _facilityAdminService.AddFacility("Test Facility");
                facilities = _facilityAdminService.GetAllFacilities();
            }

            IList<Location> locations = _locationAdminService.GetAllLocations();
            if (locations.Count == 0)
            {
                Location location = new Location();

                location.Active = true;
                location.Bed = "Bed";
                location.Building = "Building";
                location.Facility = facilities[0];
                location.Floor = "Floor";
                location.InactiveDate = Platform.Time;
                location.PointOfCare = "Point of Care";
                location.Room = "Room";

                _locationAdminService.AddLocation(location);

                locations = _locationAdminService.GetAllLocations();
            }

            VisitLocation vl = new VisitLocation();

            vl.Role = VisitLocationRole.CR;
            vl.Location = locations[0];
            vl.StartTime = Platform.Time;
            vl.EndTime = null;

            _visit.Locations.Add(vl);
        }

        public void UpdateSelectedVisitLocation()
        {
            StubUpdateSelectedVisitLocaiont();

            LoadLocations();
            this.Modified = true;
        }

        private void StubUpdateSelectedVisitLocaiont()
        {
            VisitLocation vl = _currentVisitLocationSelection;

            vl.Role = VisitLocationRole.PR;
            vl.EndTime = Platform.Time;
        }

        public void DeleteSelectedVisitLocation()
        {
            _visit.Locations.Remove(_currentVisitLocationSelection);

            LoadLocations();
            this.Modified = true;
        }

        public void LoadLocations()
        {
            _locationsTable.Items.Clear();
            _locationsTable.Items.AddRange(_visit.Locations);
        }
    }
}
