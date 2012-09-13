#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
    public class PatientBiographyDocument : Document
    {
		private readonly EntityRef _patientRef;
		private readonly EntityRef _profileRef;
    	private readonly EntityRef _orderRef;
    	private readonly PersonNameDetail _patientName;
    	private readonly CompositeIdentifierDetail _mrn;

		public PatientBiographyDocument(PatientProfileSummary patientProfile, IDesktopWindow window)
			: base(patientProfile.PatientRef, window)
		{
			Platform.CheckForNullReference(patientProfile.PatientRef, "PatientRef");
			Platform.CheckForNullReference(patientProfile.PatientProfileRef, "PatientProfileRef");

			_patientRef = patientProfile.PatientRef;
			_profileRef = patientProfile.PatientProfileRef;
			_patientName = patientProfile.Name;
			_mrn = patientProfile.Mrn;
		}

		public PatientBiographyDocument(WorklistItemSummaryBase worklistItem, IDesktopWindow window)
			: base(worklistItem.PatientRef, window)
        {
			Platform.CheckForNullReference(worklistItem.PatientRef, "PatientRef");
			Platform.CheckForNullReference(worklistItem.PatientProfileRef, "PatientProfileRef");
			Platform.CheckForNullReference(worklistItem.OrderRef, "OrderRef");

			_patientRef = worklistItem.PatientRef;
			_profileRef = worklistItem.PatientProfileRef;
			_orderRef = worklistItem.OrderRef;
			_patientName = worklistItem.PatientName;
			_mrn = worklistItem.Mrn;
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

    	public override OpenWorkspaceOperationAuditData GetAuditData()
    	{
			return new OpenWorkspaceOperationAuditData("Biography", _mrn, _patientName);
    	}
    }    
}
