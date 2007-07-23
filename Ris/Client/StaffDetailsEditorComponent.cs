using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Application.Common;

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
        private bool _isNew;
        private IList<EnumValueInfo> _staffTypeChoices;

        /// <summary>
        /// Constructor
        /// </summary>
        public StaffDetailsEditorComponent(bool isNew, IList<EnumValueInfo> staffTypeChoices)
        {
            _staffDetail = new StaffDetail();
            _isNew = isNew;
            _staffTypeChoices = staffTypeChoices;
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
            }
        }

        #region Presentation Model

        public string StaffType
        {
            get { return _staffDetail.StaffType.Value; }
            set
            {
                _staffDetail.StaffType = EnumValueUtils.MapDisplayValue(_staffTypeChoices, value);

                this.Modified = true;

                // this may have affected whether this is a physician or not
                NotifyPropertyChanged("IsPractitioner");
            }
        }

        public List<string> StaffTypeChoices
        {
            get { return EnumValueUtils.GetDisplayValues(_staffTypeChoices); }
        }

        public bool IsPractitioner
        {
            get
            {
                // JR: this is a really crappy hack but I'm going to fix it later
                // and if I don't, then someone should punish me
                return _staffDetail.StaffType.Code.StartsWith("R");
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
