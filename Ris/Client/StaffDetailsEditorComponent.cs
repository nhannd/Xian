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
            }
        }

        public List<string> StaffTypeChoices
        {
            get { return EnumValueUtils.GetDisplayValues(_staffTypeChoices); }
        }

        public string StaffId
        {
            get { return _staffDetail.StaffId; }
            set
            {
                _staffDetail.StaffId = value;
                this.Modified = true;
            }
        }

        public string FamilyName
        {
            get { return _staffDetail.Name.FamilyName; }
            set 
            {
                _staffDetail.Name.FamilyName = value;
                this.Modified = true;
            }
        }

        public string GivenName
        {
            get { return _staffDetail.Name.GivenName; }
            set
            {
                _staffDetail.Name.GivenName = value;
                this.Modified = true;
            }
        }

        public string MiddleName
        {
            get { return _staffDetail.Name.MiddleName; }
            set
            {
                _staffDetail.Name.MiddleName = value;
                this.Modified = true;
            }
        }

        public string Prefix
        {
            get { return _staffDetail.Name.Prefix; }
            set
            {
                _staffDetail.Name.Prefix = value;
                this.Modified = true;
            }
        }

        public string Suffix
        {
            get { return _staffDetail.Name.Suffix; }
            set
            {
                _staffDetail.Name.Suffix = value;
                this.Modified = true;
            }
        }

        public string Degree
        {
            get { return _staffDetail.Name.Degree; }
            set
            {
                _staffDetail.Name.Degree = value;
                this.Modified = true;
            }
        }

        #endregion
    }
}
