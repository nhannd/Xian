#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// Extension point for views onto <see cref="ExternalPractitionerDetailsEditorComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class ExternalPractitionerDetailsEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// ExternalPractitionerDetailsEditorComponent class
    /// </summary>
    [AssociateView(typeof(ExternalPractitionerDetailsEditorComponentViewExtensionPoint))]
    public class ExternalPractitionerDetailsEditorComponent : ApplicationComponent
    {
        private ExternalPractitionerDetail _practitionerDetail;
        private bool _isNew;
    	private bool _markVerified;

        /// <summary>
        /// Constructor
        /// </summary>
        public ExternalPractitionerDetailsEditorComponent(bool isNew)
        {
            _practitionerDetail = new ExternalPractitionerDetail();
            _isNew = isNew;
        }

        public ExternalPractitionerDetail ExternalPractitionerDetail
        {
            get { return _practitionerDetail; }
            set 
            { 
                _practitionerDetail = value;
            	_markVerified = _practitionerDetail.IsVerified;
            }
        }

        #region Presentation Model

        [ValidateNotNull]
        public string FamilyName
        {
            get { return _practitionerDetail.Name.FamilyName; }
            set 
            {
                _practitionerDetail.Name.FamilyName = value;
                this.Modified = true;
            }
        }

        [ValidateNotNull]
        public string GivenName
        {
            get { return _practitionerDetail.Name.GivenName; }
            set
            {
                _practitionerDetail.Name.GivenName = value;
                this.Modified = true;
            }
        }

        public string MiddleName
        {
            get { return _practitionerDetail.Name.MiddleName; }
            set
            {
                _practitionerDetail.Name.MiddleName = value;
                this.Modified = true;
            }
        }

        public string Prefix
        {
            get { return _practitionerDetail.Name.Prefix; }
            set
            {
                _practitionerDetail.Name.Prefix = value;
                this.Modified = true;
            }
        }

        public string Suffix
        {
            get { return _practitionerDetail.Name.Suffix; }
            set
            {
                _practitionerDetail.Name.Suffix = value;
                this.Modified = true;
            }
        }

        public string Degree
        {
            get { return _practitionerDetail.Name.Degree; }
            set
            {
                _practitionerDetail.Name.Degree = value;
                this.Modified = true;
            }
        }

        public string LicenseNumber
        {
            get { return _practitionerDetail.LicenseNumber; }
            set
            {
                _practitionerDetail.LicenseNumber = value;
                this.Modified = true;
            }
        }

        public string BillingNumber
        {
            get { return _practitionerDetail.BillingNumber; }
            set
            {
                _practitionerDetail.BillingNumber = value;
                this.Modified = true;
            }
        }

		public bool MarkVerified
		{
			get { return _markVerified; }
			set
			{
				_markVerified = value;
				this.Modified = true;
			}
		}

		public string LastVerified
		{
			get
			{
				var lastVerified = _practitionerDetail.LastVerifiedTime == null ? SR.MessageNever : Format.DateTime(_practitionerDetail.LastVerifiedTime);
				return string.Format(SR.FormatLastVerified, lastVerified);
			}
		}

		public bool CanVerify
		{
			get { return Thread.CurrentPrincipal.IsInRole(Application.Common.AuthorityTokens.Admin.Data.ExternalPractitionerVerification); }
		}

		#endregion
	}
}
