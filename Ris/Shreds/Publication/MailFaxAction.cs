#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Workflow;

namespace ClearCanvas.Ris.Shreds.Publication
{
	/// <summary>
    /// This publication action sends the report to the outbound fax/mail queue.
    /// </summary>
	// JR: Disabled as of Yen - this action is not used
	//[ExtensionOf(typeof(PublicationActionExtensionPoint))]
	public class MailFaxAction : IPublicationAction
	{
		#region IPublicationAction Members

		public void Execute(ReportPart reportPart, IPersistenceContext context)
		{
			var order = reportPart.Report.Procedures.First().Order;
			foreach (var recipient in order.ResultRecipients)
			{
				var item = Create(
					order.AccessionNumber,
					reportPart.Report.GetRef(),
					recipient.PractitionerContactPoint.Practitioner.GetRef(),
					recipient.PractitionerContactPoint.GetRef());

				context.Lock(item, DirtyState.New);
			}
		}

		#endregion

		private static WorkQueueItem Create(
			string accessionNumber,
			EntityRef reportRef,
			EntityRef practitionerRef,
			EntityRef contactPointRef)
		{
			var workQueueItem = new WorkQueueItem("Mail/Fax Report");
			workQueueItem.ExtendedProperties.Add("AccessionNumber", accessionNumber);
			workQueueItem.ExtendedProperties.Add("ReportOID", reportRef.ToString(false, false));
			workQueueItem.ExtendedProperties.Add("ExternalPractitionerOID", practitionerRef.ToString(false, false));
			workQueueItem.ExtendedProperties.Add("ExternalPractitionerContactPointOID", contactPointRef.ToString(false, false));

			return workQueueItem;
		}
	}
}
