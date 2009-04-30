#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.LocationAdmin;
using ClearCanvas.Ris.Application.Common.Admin.FacilityAdmin;
using ClearCanvas.Desktop.Validation;

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
        private List<FacilitySummary> _facilityChoices;

        private LocationDetail _locationDetail;
        private EntityRef _locationRef;
        private bool _isNew;

        private LocationSummary _locationSummary;

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

        public LocationSummary LocationSummary
        {
            get { return _locationSummary; }
        }

        public override void Start()
        {
            if (_isNew)
            {
                _locationDetail = new LocationDetail();
            }
            else
            {
                Platform.GetService<ILocationAdminService>(
                    delegate(ILocationAdminService service)
                    {
                            LoadLocationForEditResponse response = service.LoadLocationForEdit(new LoadLocationForEditRequest(_locationRef));
                            _locationRef = response.LocationDetail.LocationRef;
                            _locationDetail = response.LocationDetail;
                    });
            }
        
            Platform.GetService<IFacilityAdminService>(
                delegate(IFacilityAdminService service)
                {
                    ListAllFacilitiesResponse response = service.ListAllFacilities(new ListAllFacilitiesRequest());
                    _facilityChoices = response.Facilities;

                    if (_isNew && _locationDetail.Facility == null && response.Facilities.Count > 0)
                    {
                        _locationDetail.Facility = response.Facilities[0];
                    }
                });
                

            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
        }

        public LocationDetail LocationDetail
        {
            get { return _locationDetail; }
            set { _locationDetail = value; }
        }

        #region Presentation Model

        [ValidateNotNull]
        public string Id
		{
			get { return _locationDetail.Id; }
			set
			{
				_locationDetail.Id = value;
				this.Modified = true;
			}
		}

        [ValidateNotNull]
        public string Name
		{
			get { return _locationDetail.Name; }
			set
			{
				_locationDetail.Name = value;
				this.Modified = true;
			}
		}

		public string Description
		{
			get { return _locationDetail.Description; }
			set
			{
				_locationDetail.Description = value;
				this.Modified = true;
			}
		}

		public IList FacilityChoices
        {
            get { return _facilityChoices; }
        }

        [ValidateNotNull]
        public FacilitySummary Facility
        {
            get { return _locationDetail.Facility; }
            set
            {
            	_locationDetail.Facility = value;
                this.Modified = true;
            }
        }

		public string FormatFacility(object item)
		{
			FacilitySummary f = (FacilitySummary) item;
			return f.Name;
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

                    this.Exit(ApplicationComponentExitCode.Accepted);
                }
                catch (Exception e)
                {
                    ExceptionHandler.Report(e, SR.ExceptionSaveLocation, this.Host.DesktopWindow,
                        delegate()
                        {
                            this.ExitCode = ApplicationComponentExitCode.Error;
                            this.Host.Exit();
                        });
                }
            }
        }

        public void Cancel()
        {
            this.ExitCode = ApplicationComponentExitCode.None;
            Host.Exit();
        }

        public bool AcceptEnabled
        {
            get { return this.Modified; }
        }

        #endregion

        private void SaveChanges()
        {
            if (_isNew)
            {
                Platform.GetService<ILocationAdminService>(
                    delegate(ILocationAdminService service)
                    {
                        AddLocationResponse response = service.AddLocation(new AddLocationRequest(_locationDetail));
                        _locationRef = response.Location.LocationRef;
                        _locationSummary = response.Location;
                    });
            }
            else
            {
                Platform.GetService<ILocationAdminService>(
                    delegate(ILocationAdminService service)
                    {
                        UpdateLocationResponse response = service.UpdateLocation(new UpdateLocationRequest(_locationDetail));
                        _locationRef = response.Location.LocationRef;
                        _locationSummary = response.Location;
                    });
            }
        }

        public event EventHandler AcceptEnabledChanged
        {
            add { this.ModifiedChanged += value; }
            remove { this.ModifiedChanged -= value; }
        }
    }
}
