#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
	[DataContract]
	public class SendReportToQueueRequest : DataContractBase
	{
		public SendReportToQueueRequest(EntityRef procedureRef)
		{
			this.ProcedureRef = procedureRef;
			this.Recipients = new List<PublishRecipientDetail>();
		}

		[DataMember]
		public EntityRef ProcedureRef;

		[DataMember]
		public List<PublishRecipientDetail> Recipients;
	}

	[DataContract]
	public class PublishRecipientDetail : DataContractBase
	{
		public PublishRecipientDetail(EntityRef practitionerRef, EntityRef contactPointRef)
		{
			this.PractitionerRef = practitionerRef;
			this.ContactPointRef = contactPointRef;
		}

		[DataMember]
		public EntityRef PractitionerRef;

		[DataMember]
		public EntityRef ContactPointRef; 
	}
}