using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Application.Common.Admin.LocationAdmin;
using ClearCanvas.Ris.Application.Common.Admin.FacilityAdmin;

namespace ClearCanvas.Ris.Client.Admin
{
    /// <summary>
    /// Extension point for views onto <see cref="LocationEditorComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class LocationEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// LocationEditorComponent class
    /// </summary>
    [AssociateView(typeof(LocationEditorComponentViewExtensionPoint))]
    public class LocationEditorComponent : ApplicationComponent
    {
        private LocationDetail _locationDetail;
        private EntityRef _locationRef;
        private bool _isNew;

        /// <summary>
        /// Constructor
        /// </summary>
        public LocationEditorComponent()
        {
            _isNew = true;
        }

        public LocationEditorComponent(EntityRef locationRef)
        {
            _isNew = false;
            _locationRef = locationRef;
        }

        public override void Start()
        {
            if (_isNew)
            {
                _locationDetail = new LocationDetail();
            }
            else
            {
                try
                {
                    Platform.GetService<ILocationAdminService>(
                        delegate(ILocationAdminService service)
                        {
                            LoadLocationForEditResponse response = service.LoadLocationForEdit(new LoadLocationForEditRequest(_locationRef));
                            _locationRef = response.LocationRef;
                            _locationDetail = response.LocationDetail;
                        });
                }
                catch (Exception e)
                {
                    ExceptionHandler.Report(e, this.Host.DesktopWindow);
                }
            }

            base.Start();
        }

        public override void Stop()
        {
            // TODO prepare the component to exit the live phase
            // This is a good place to do any clean up
            base.Stop();
        }

        public LocationDetail LocationDetail
        {
            get { return _locationDetail; }
            set { _locationDetail = value; }
        }

        public IList FacilityChoices
        {
            get 
            {
                try
                {
                    List<string> facilityStrings = new List<string>();

                    Platform.GetService<IFacilityAdminService>(
                        delegate(IFacilityAdminService service)
                        {
                            ListAllFacilitiesResponse response = service.ListAllLocations(new ListAllFacilitiesRequest());
                            facilityStrings.AddRange(
                                CollectionUtils.Map<FacilitySummary, string>(response.Facilities,
                                        delegate(FacilitySummary f) { return f.Name; }));

                            if (_isNew && _locationDetail.Facility == null && response.Facilities.Count > 0)
                            {
                                _locationDetail.Facility = response.Facilities[0].FacilityRef;
                            }
                        
                        });

                    return facilityStrings;
                }
                catch (Exception e)
                {
                    ExceptionHandler.Report(e, this.Host.DesktopWindow);
                }
            }
        }

        public string Facility
        {
            get { return (_locationDetail.Facility == null ? "" : _locationDetail.FacilityName); }
            set
            {
                try
                {
                    Platform.GetService<IFacilityAdminService>(
                        delegate(IFacilityAdminService service)
                        {
                            ListAllFacilitiesResponse response = service.ListAllLocations(new ListAllFacilitiesRequest());
                            FacilitySummary summary = CollectionUtils.SelectFirst<FacilitySummary>(response.Facilities,
                                    delegate(FacilitySummary f) { return f.Name == value; });

                            if (summary != null)
                            {
                                _locationDetail.Facility = summary.FacilityRef;
                                _locationDetail.FacilityName = summary.Name;
                            }
                        });
                    

                    this.Modified = true;
                }
                catch (Exception e)
                {
                    ExceptionHandler.Report(e, this.Host.DesktopWindow);
                }
            }
        }

        public string Building
        {
            get { return _locationDetail.Building; }
            set 
            { 
                _locationDetail.Building = value;
                this.Modified = true;
            }
        }

        public string Floor
        {
            get { return _locationDetail.Floor; }
            set 
            { 
                _locationDetail.Floor = value;
                this.Modified = true;
            }
        }

        public string PointOfCare
        {
            get { return _locationDetail.PointOfCare; }
            set 
            { 
                _locationDetail.PointOfCare = value;
                this.Modified = true;
            }
        }

        public string Room
        {
            get { return _locationDetail.Room; }
            set 
            { 
                _locationDetail.Room = value;
                this.Modified = true;
            }
        }

        public string Bed
        {
            get { return _locationDetail.Bed; }
            set 
            { 
                _locationDetail.Bed = value;
                this.Modified = true;
            }
        }

        public void Accept()
        {
            if (this.HasValidationErrors)
            {
                this.ShowValidation(true);
            }
            else
            {
                try
                {
                    SaveChanges();
                    this.ExitCode = ApplicationComponentExitCode.Normal;
                    Host.Exit();
                }
                catch (ConcurrencyException e)
                {
                    ExceptionHandler.Report(e, SR.ExceptionConcurrencyLocationNotSaved, this.Host.DesktopWindow);
                }
                catch (Exception e)
                {
                    ExceptionHandler.Report(e, this.Host.DesktopWindow);
                }
            }
        }

        private void SaveChanges()
        {
            try
            {
                if (_isNew)
                {
                    Platform.GetService<ILocationAdminService>(
                        delegate(ILocationAdminService service)
                        {
                            AddLocationResponse response = service.AddLocation(new AddLocationRequest(_locationDetail));
                            _locationRef = response.Location.LocationRef;
                        });
                }
                else
                {
                    Platform.GetService<ILocationAdminService>(
                        delegate(ILocationAdminService service)
                        {
                            UpdateLocationResponse response = service.UpdateLocation(new UpdateLocationRequest(_locationRef, _locationDetail));
                            _locationRef = response.Location.LocationRef;
                        });
                }
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        public void Cancel()
        {
            this.ExitCode = ApplicationComponentExitCode.Cancelled;
            Host.Exit();
        }

        public bool AcceptEnabled
        {
            get { return this.Modified; }
        }

        public event EventHandler AcceptEnabledChanged
        {
            add { this.ModifiedChanged += value; }
            remove { this.ModifiedChanged -= value; }
        }
    }
}
