using System.Collections.Generic;
using ClearCanvas.Common.Shreds;

namespace ClearCanvas.Ris.Shreds.ImageAvailability
{
	//[ExtensionOf(typeof(ShredExtensionPoint))]
	public class ImageAvailabilityShred : QueueProcessorShred
	{
		public ImageAvailabilityShred()
		{
		}

		public override string GetDisplayName()
		{
			return SR.ImageAvailabilityShredName;
		}

		public override string GetDescription()
		{
			return SR.ImageAvailabilityShredDescription;
		}

		protected override IList<QueueProcessor> GetProcessors()
		{
            ImageAvailabilityShredSettings settings = new ImageAvailabilityShredSettings();
			QueueProcessor[] processors = new QueueProcessor[] {
                new ImageAvailabilityProcedureProcessor(settings),
                new ImageAvailabilityWorkQueueProcessor(settings)
            };

			return processors;
		}
	}
}
