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
    /// Extension point for views onto <see cref="PractitionerStaffDetailsEditorComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class PractitionerStaffDetailsEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// PractitionerStaffDetailsEditorComponent class
    /// </summary>
    [AssociateView(typeof(PractitionerStaffDetailsEditorComponentViewExtensionPoint))]
    public class PractitionerStaffDetailsEditorComponent : ApplicationComponent
    {
        private Practitioner _practitioner;
        private Staff _staff;
        private bool _isStaffMode;

        /// <summary>
        /// Constructor
        /// </summary>
        public PractitionerStaffDetailsEditorComponent()
        {
            _practitioner = new Practitioner();
            _staff = new Staff();
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
            set 
            {
                _staff = value;
                _isStaffMode = true;
            }
        }

        public Practitioner Practitioner
        {
            get { return _practitioner; }
            set 
            {
                _practitioner = value;
                _staff = _practitioner as Staff;
                _isStaffMode = false;
            }
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
                    return _practitioner.LicenseNumber;
            }

            set
            {
                if (_isStaffMode == false)
                {
                    _practitioner.LicenseNumber = value;
                    this.Modified = true;
                }
            }
        }

        #endregion
    }
}
