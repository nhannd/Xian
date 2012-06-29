#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Client.Workflow
{
	public class ReportingOrderDetailViewComponent : OrderDetailViewComponent
	{
        // Internal data contract used for jscript deserialization
        [DataContract]
        public class PatientOrderContext : DataContractBase
        {
            public PatientOrderContext(EntityRef patientRef, EntityRef orderRef)
            {
                this.PatientRef = patientRef;
                this.OrderRef = orderRef;
            }

            [DataMember]
            public EntityRef PatientRef;

            [DataMember]
            public EntityRef OrderRef;
        }

		public ReportingOrderDetailViewComponent()
		{
		}

		public ReportingOrderDetailViewComponent(EntityRef patientRef, EntityRef orderRef)
		{
            _context = new PatientOrderContext(patientRef, orderRef);
        }

        protected override string PageUrl
		{
			get { return WebResourcesSettings.Default.ReportingOrderDetailPageUrl; }
		}
	}
}
