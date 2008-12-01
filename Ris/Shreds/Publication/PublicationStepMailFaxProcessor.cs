using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;

namespace ClearCanvas.Ris.Shreds.Publication
{
	[ExtensionOf(typeof(PublicationStepProcessorExtensionPoint))]
	public class PublicationStepMailFaxProcessor : IPublicationStepProcessor
	{
		#region IPublicationStepProcessor Members

		public void Process(PublicationStep step, IPersistenceContext context)
		{
			foreach (ResultRecipient recipient in step.Procedure.Order.ResultRecipients)
			{
				MailFaxWorkQueueItem.Schedule(
					step.Procedure.Order.AccessionNumber,
					step.ReportPart.Report.GetRef(),
					recipient.PractitionerContactPoint.Practitioner.GetRef(),
					recipient.PractitionerContactPoint.GetRef(),
					context);
			}
		}

		#endregion
	}
}
