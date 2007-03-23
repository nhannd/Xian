using System;
using System.Collections.Generic;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Ris.Client;
using ClearCanvas.Ris.Application.Common.Admin.VisitAdmin;
using ClearCanvas.Ris.Application.Common.Admin.FacilityAdmin;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Application.Common.Admin.LocationAdmin;

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
        private VisitDetail _visit;
        private VisitLocationTable _locationsTable;
        private VisitLocationDetail _currentVisitLocationSelection;
        private List<EnumValueInfo> _visitLocationRoleChoices;
        private CrudActionModel _visitLocationActionHandler;

        /// <summary>
        /// Constructor
        /// </summary>
        public VisitLocationsSummaryComponent(List<EnumValueInfo> visitLocationRoleChoices)
        {
            _locationsTable = new VisitLocationTable();
            _visitLocationRoleChoices = visitLocationRoleChoices;

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

        public VisitDetail Visit
        {
            get { return _visit; }
            set { _visit = value; }
        }

        public ITable Locations
        {
            get { return _locationsTable; }
        }

        public VisitLocationDetail CurrentVisitLocationSelection
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
            this.CurrentVisitLocationSelection = (VisitLocationDetail)selection.Item;
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
            DummyAddVisitLocation();
            
            LoadLocations();
            this.Modified = true;
        }

        public void UpdateSelectedVisitLocation()
        {
            DummyUpdateSelectedVisitLocation();

            LoadLocations();
            this.Modified = true;
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

        #region Dummy Code

        private void DummyAddVisitLocation()
        {
            try
            {
                FacilitySummary facility = new FacilitySummary();
                LocationSummary location = new LocationSummary();

                Platform.GetService<IFacilityAdminService>(
                    delegate(IFacilityAdminService service)
                    {
                        ListAllFacilitiesResponse listResponse = service.ListAllFacilities(new ListAllFacilitiesRequest());
                        if (listResponse.Facilities.Count == 0)
                        {
                            AddFacilityResponse addResponse = service.AddFacility(new AddFacilityRequest(new FacilityDetail("", "Test Facility")));
                            facility = addResponse.Facility;
                        }
                        else
                        {
                            facility = listResponse.Facilities[0];
                        }
                    });

                Platform.GetService<ILocationAdminService>(
                    delegate(ILocationAdminService service)
                    {
                        ListAllLocationsResponse listResponse= service.ListAllLocations(new ListAllLocationsRequest(true));
                        if (listResponse.Locations.Count == 0)
                        {
                            LocationDetail locationDetail = new LocationDetail(
                                    facility.FacilityRef,
                                    facility.Name,
                                    facility.Code,
                                    "Building",
                                    "Floor",
                                    "Point of Care",
                                    "Room",
                                    "Bed");

                            AddLocationResponse response = service.AddLocation(new AddLocationRequest(locationDetail));
                            location = response.Location;
                        }
                        else
                        {
                            location = listResponse.Locations[0];
                        }
                    });

                VisitLocationDetail vl = new VisitLocationDetail();

                vl.Role = CollectionUtils.SelectFirst<EnumValueInfo>(_visitLocationRoleChoices,
                        delegate(EnumValueInfo e) { return e.Code == "CR"; });
                vl.Location = location;
                vl.StartTime = Platform.Time;
                vl.EndTime = null;

                _visit.Locations.Add(vl);
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        private void DummyUpdateSelectedVisitLocation()
        {
            VisitLocationDetail vl = _currentVisitLocationSelection;

            vl.Role = CollectionUtils.SelectFirst<EnumValueInfo>(_visitLocationRoleChoices,
                delegate(EnumValueInfo e) { return e.Code == "PR"; });
            vl.EndTime = Platform.Time;
        }

        #endregion
    }
}
