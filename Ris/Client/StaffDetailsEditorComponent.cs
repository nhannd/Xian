using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common.Admin;

namespace ClearCanvas.Ris.Client
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
        private bool _isPractitioner;
        private bool _isNew;

        /// <summary>
        /// Constructor
        /// </summary>
        public StaffDetailsEditorComponent(bool isNew)
        {
            _staffDetail = new StaffDetail();
            _isNew = isNew;
        }

        public override void Start()
        {
            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
        }

        public StaffDetail StaffDetail
        {
            get { return _staffDetail; }
            set 
            { 
                _staffDetail = value;
                if (_staffDetail.LicenseNumber != null && _staffDetail.LicenseNumber != "")
                    this.IsPractitioner = true;
            }
        }

        #region Presentation Model

        public bool NewStaff
        {
            get { return _isNew; }
        }

        public bool IsPractitioner
        {
            get { return _isPractitioner; }
            set 
            {
                _isPractitioner = value;
                if (_isPractitioner == false)
                    _staffDetail.LicenseNumber = "";
            }
        }

        public string FamilyName
        {
            get { return _staffDetail.PersonNameDetail.FamilyName; }
            set 
            {
                _staffDetail.PersonNameDetail.FamilyName = value;
                this.Modified = true;
            }
        }

        public string GivenName
        {
            get { return _staffDetail.PersonNameDetail.GivenName; }
            set
            {
                _staffDetail.PersonNameDetail.GivenName = value;
                this.Modified = true;
            }
        }

        public string MiddleName
        {
            get { return _staffDetail.PersonNameDetail.MiddleName; }
            set
            {
                _staffDetail.PersonNameDetail.MiddleName = value;
                this.Modified = true;
            }
        }

        public string Prefix
        {
            get { return _staffDetail.PersonNameDetail.Prefix; }
            set
            {
                _staffDetail.PersonNameDetail.Prefix = value;
                this.Modified = true;
            }
        }

        public string Suffix
        {
            get { return _staffDetail.PersonNameDetail.Suffix; }
            set
            {
                _staffDetail.PersonNameDetail.Suffix = value;
                this.Modified = true;
            }
        }

        public string Degree
        {
            get { return _staffDetail.PersonNameDetail.Degree; }
            set
            {
                _staffDetail.PersonNameDetail.Degree = value;
                this.Modified = true;
            }
        }

        public string LicenseNumber
        {
            get { return _staffDetail.LicenseNumber; }
            set
            {
                _staffDetail.LicenseNumber = value;
                this.Modified = true;
            }
        }

        #endregion
    }
}
