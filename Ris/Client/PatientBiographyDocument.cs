#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Client
{
    public class PatientBiographyDocument : Document
    {
		private readonly EntityRef _patientRef;
		private readonly EntityRef _profileRef;
    	private readonly EntityRef _orderRef;

		public PatientBiographyDocument(EntityRef patientRef, EntityRef profileRef, IDesktopWindow window)
			: base(patientRef, window)
		{
			Platform.CheckForNullReference(patientRef, "patientRef");
			Platform.CheckForNullReference(profileRef, "profileRef");

			_patientRef = patientRef;
			_profileRef = profileRef;
		}

		public PatientBiographyDocument(EntityRef patientRef, EntityRef profileRef, EntityRef orderRef, IDesktopWindow window)
			: this(patientRef, profileRef, window)
        {
			_orderRef = orderRef;
		}

        public override string GetTitle()
        {
            return "";  // not relevant - component will set title
        }

		public override bool SaveAndClose()
		{
			return base.Close();
		}

        public override IApplicationComponent GetComponent()
        {
			var component = new BiographyOverviewComponent(_patientRef, _profileRef, _orderRef);
        	return component;
        }
    }    
}
