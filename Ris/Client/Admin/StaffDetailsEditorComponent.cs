using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise;
using ClearCanvas.Desktop;

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
        private Staff _staff;
        private bool _isStaffMode;

        /// <summary>
        /// Constructor
        /// </summary>
        public StaffDetailsEditorComponent(bool staffMode)
        {
            _isStaffMode = staffMode;
            if (_isStaffMode)
                _staff = new Staff();
            else
                _staff = new Practitioner();
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

        public Staff Staff
        {
            get { return _staff; }
            set { _staff = value; }
        }

        public bool StaffMode
        {
            get { return _isStaffMode; }
        }

        #region Staff Presentation Model

        public string FamilyName
        {
            get { return _staff.Name.FamilyName; }
            set
            {
                _staff.Name.FamilyName = value;
                this.Modified = true;
            }
        }

        public string GivenName
        {
            get { return _staff.Name.GivenName; }
            set
            {
                _staff.Name.GivenName = value;
                this.Modified = true;
            }
        }

        public string MiddleName
        {
            get { return _staff.Name.MiddleName; }
            set
            {
                _staff.Name.MiddleName = value;
                this.Modified = true;
            }
        }

        public string Prefix
        {
            get { return _staff.Name.Prefix; }
            set
            {
                _staff.Name.Prefix = value;
                this.Modified = true;
            }
        }

        public string Suffix
        {
            get { return _staff.Name.Suffix; }
            set
            {
                _staff.Name.Suffix = value;
                this.Modified = true;
            }
        }

        public string Degree
        {
            get { return _staff.Name.Degree; }
            set
            {
                _staff.Name.Degree = value;
                this.Modified = true;
            }
        }

        #endregion

        #region Practitioner Presentation Model

        public string LicenseNumber
        {
            get
            {
                if (_isStaffMode)
                    return null;
                else
                    return (_staff as Practitioner).LicenseNumber;
            }

            set
            {
                if (_isStaffMode == false)
                {
                    (_staff as Practitioner).LicenseNumber = value;
                    this.Modified = true;
                }
            }
        }

        #endregion
    }
}
