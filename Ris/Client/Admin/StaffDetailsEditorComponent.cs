using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common.Admin;

namespace ClearCanvas.Ris.Client.Admin
{
    /// <summary>
    /// Extension point for views onto <see cref="StaffDetailsEditorComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class StaffDetailsEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// StaffDetailsEditorComponent class
    /// </summary>
    [AssociateView(typeof(StaffDetailsEditorComponentViewExtensionPoint))]
    public class StaffDetailsEditorComponent : ApplicationComponent
    {
        private StaffDetail _staffDetail;
        private PractitionerDetail _practitionerDetail;
        private bool _isStaffMode;

        /// <summary>
        /// Constructor
        /// </summary>
        public StaffDetailsEditorComponent(bool staffMode)
        {
            _isStaffMode = staffMode;
            if (_isStaffMode)
                _staffDetail = new StaffDetail();
            else
                _practitionerDetail = new PractitionerDetail();
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

        public StaffDetail StaffDetail
        {
            get { return _staffDetail; }
            set { _staffDetail = value; }
        }

        public PractitionerDetail PractitionerDetail
        {
            get { return _practitionerDetail; }
            set { _practitionerDetail = value; }
        }

        public bool StaffMode
        {
            get { return _isStaffMode; }
        }

        #region Staff Presentation Model

        public string FamilyName
        {
            get 
            {
                return (_isStaffMode ? _staffDetail.PersonNameDetail.FamilyName 
                              : _practitionerDetail.PersonNameDetail.FamilyName);
            }
            set
            {
                if (_isStaffMode)
                    _staffDetail.PersonNameDetail.FamilyName = value;
                else
                    _practitionerDetail.PersonNameDetail.FamilyName = value;

                this.Modified = true;
            }
        }

        public string GivenName
        {
            get
            {
                return (_isStaffMode ? _staffDetail.PersonNameDetail.GivenName
                              : _practitionerDetail.PersonNameDetail.GivenName);
            }
            set
            {
                if (_isStaffMode)
                    _staffDetail.PersonNameDetail.GivenName = value;
                else
                    _practitionerDetail.PersonNameDetail.GivenName = value;

                this.Modified = true;
            }
        }

        public string MiddleName
        {
            get
            {
                return (_isStaffMode ? _staffDetail.PersonNameDetail.MiddleName
                              : _practitionerDetail.PersonNameDetail.MiddleName);
            }
            set
            {
                if (_isStaffMode)
                    _staffDetail.PersonNameDetail.MiddleName = value;
                else
                    _practitionerDetail.PersonNameDetail.MiddleName = value;

                this.Modified = true;
            }
        }

        public string Prefix
        {
            get
            {
                return (_isStaffMode ? _staffDetail.PersonNameDetail.Prefix
                              : _practitionerDetail.PersonNameDetail.Prefix);
            }
            set
            {
                if (_isStaffMode)
                    _staffDetail.PersonNameDetail.Prefix = value;
                else
                    _practitionerDetail.PersonNameDetail.Prefix = value;

                this.Modified = true;
            }
        }

        public string Suffix
        {
            get
            {
                return (_isStaffMode ? _staffDetail.PersonNameDetail.Suffix
                              : _practitionerDetail.PersonNameDetail.Suffix);
            }
            set
            {
                if (_isStaffMode)
                    _staffDetail.PersonNameDetail.Suffix = value;
                else
                    _practitionerDetail.PersonNameDetail.Suffix = value;

                this.Modified = true;
            }
        }

        public string Degree
        {
            get
            {
                return (_isStaffMode ? _staffDetail.PersonNameDetail.Degree
                              : _practitionerDetail.PersonNameDetail.Degree);
            }
            set
            {
                if (_isStaffMode)
                    _staffDetail.PersonNameDetail.Degree = value;
                else
                    _practitionerDetail.PersonNameDetail.Degree = value;

                this.Modified = true;
            }
        }

        #endregion

        #region Practitioner Presentation Model

        public string LicenseNumber
        {
            get
            {
                return (_isStaffMode ? "" : _practitionerDetail.LicenseNumber);
            }
            set
            {
                if (_isStaffMode == false)
                {
                    _practitionerDetail.LicenseNumber = value;
                    this.Modified = true;
                }
            }
        }

        #endregion
    }
}
