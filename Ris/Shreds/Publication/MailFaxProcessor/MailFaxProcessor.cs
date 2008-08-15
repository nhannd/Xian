using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;

namespace ClearCanvas.Ris.Shreds.Publication.MailFaxProcessor
{
	[ExtensionOf(typeof(PublicationStepProcessorExtensionPoint))]
	public class MailFaxProcessor : IPublicationStepProcessor
	{
		#region IPublicationStepProcessor Members

		public void Process(PublicationStep step, IPersistenceContext context)
		{
			foreach (ResultRecipient recipient in step.Procedure.Order.ResultRecipients)
			{
				MailFaxWorkQueueItem.Schedule(
					step.ReportPart.Report.GetRef(),
					recipient.PractitionerContactPoint.Practitioner.GetRef(),
					recipient.PractitionerContactPoint.GetRef(),
					context);
			}
		}

		#endregion
	}
}
