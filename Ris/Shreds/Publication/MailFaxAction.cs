using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Workflow;

namespace ClearCanvas.Ris.Shreds.Publication
{
    /// <summary>
    /// This publication action sends the report to the outbound fax/mail queue.
    /// </summary>
	[ExtensionOf(typeof(PublicationActionExtensionPoint))]
	public class MailFaxAction : IPublicationAction
	{
		#region IPublicationAction Members

		public void Execute(PublicationStep step, IPersistenceContext context)
		{
			foreach (ResultRecipient recipient in step.Procedure.Order.ResultRecipients)
			{
				WorkQueueItem item = MailFaxWorkQueueItem.Create(
					step.Procedure.Order.AccessionNumber,
					step.ReportPart.Report.GetRef(),
					recipient.PractitionerContactPoint.Practitioner.GetRef(),
					recipient.PractitionerContactPoint.GetRef());

				context.Lock(item, DirtyState.New);
			}
		}

		#endregion
	}
}
