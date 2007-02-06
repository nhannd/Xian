using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Services;

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
        private Location _location;
        private EntityRef<Location> _locationRef;
        private IFacilityAdminService _facilityAdminService;
        private ILocationAdminService _locationAdminService;
        private bool _isNew;

        /// <summary>
        /// Constructor
        /// </summary>
        public LocationEditorComponent()
        {
            _isNew = true;
        }

        public LocationEditorComponent(EntityRef<Location> locationRef)
        {
            _isNew = false;
            _locationRef = locationRef;
        }

        public override void Start()
        {
            _facilityAdminService = ApplicationContext.GetService<IFacilityAdminService>();
            _locationAdminService = ApplicationContext.GetService<ILocationAdminService>();

            if (_isNew)
            {
                _location = new Location();
            }
            else
            {
                try
                {
                    _location = _locationAdminService.LoadLocation(_locationRef);
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

        public Location Location
        {
            get { return _location; }
            set { _location = value; }
        }

        public IList FacilityChoices
        {
            get 
            {
                List<string> facilityStrings = new List<string>();

                try
                {
                    IList<Facility> facilityList = _facilityAdminService.GetAllFacilities();

                    facilityStrings.AddRange(
                        CollectionUtils.Map<Facility, string>(facilityList,
                                delegate(Facility f) { return Format.Custom(f); }));

                    if (_isNew && _location.Facility == null && facilityList.Count > 0)
                        _location.Facility = facilityList[0];
                }
                catch (Exception e)
                {
                    ExceptionHandler.Report(e, this.Host.DesktopWindow);
                }

                return facilityStrings;
            }
        }

        public string Facility
        {
            get { return _location.Facility == null ? "" : Format.Custom(_location.Facility); }
            set
            {
                try
                {
                    _location.Facility = (value == "") ? null :
                        CollectionUtils.SelectFirst<Facility>(_facilityAdminService.GetAllFacilities(),
                            delegate(Facility f) { return Format.Custom(f) == value; });

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
            get { return _location.Building; }
            set 
            { 
                _location.Building = value;
                this.Modified = true;
            }
        }

        public string Floor
        {
            get { return _location.Floor; }
            set 
            { 
                _location.Floor = value;
                this.Modified = true;
            }
        }

        public string PointOfCare
        {
            get { return _location.PointOfCare; }
            set 
            { 
                _location.PointOfCare = value;
                this.Modified = true;
            }
        }

        public string Room
        {
            get { return _location.Room; }
            set 
            { 
                _location.Room = value;
                this.Modified = true;
            }
        }

        public string Bed
        {
            get { return _location.Bed; }
            set 
            { 
                _location.Bed = value;
                this.Modified = true;
            }
        }

        public bool Active
        {
            get { return _location.Active; }
            set 
            { 
                _location.Active = value;

                if (_location.Active == true && _location.InactiveDate == null)
                    InactiveDate = Platform.Time.Date;

                this.Modified = true;
            }
        }

        public DateTime? InactiveDate
        {
            get { return _location.InactiveDate; }
            set 
            { 
                _location.InactiveDate = value;
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
            if (_isNew)
            {
                _locationAdminService.AddLocation(_location);
                _locationRef = new EntityRef<Location>(_location);
            }
            else
            {
                _locationAdminService.UpdateLocation(_location);
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
