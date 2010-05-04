using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;

namespace ClearCanvas.Ris.Shreds.Publication
{
	[ExtensionOf(typeof(PublicationActionExtensionPoint))]
	public class CreateLogicalHL7EventAction : IPublicationAction
	{
		private readonly bool _enabled;

		public CreateLogicalHL7EventAction()
		{
			var settings = new PublicationShredSettings();
			_enabled = settings.HL7PublicationEnabled;
		}

		public void Execute(PublicationStep step, IPersistenceContext context)
		{
			if (_enabled == false)
				return;

			foreach (var logicalEvent in LogicalHL7EventWorkQueueItem.CreateReportLogicalEvents(LogicalHL7EventType.ReportVerified, step.Report))
			{
				context.Lock(logicalEvent.Item, DirtyState.New);
			}
		}
	}
}
