using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Server.ShredHost;

namespace ClearCanvas.Ris.Shreds.ImageAvailability
{
	//[ExtensionOf(typeof(ShredExtensionPoint))]
	public class ImageAvailabilityShred : RisShredBase
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

		protected override IList<IProcessor> GetProcessors()
		{
            ImageAvailabilityShredSettings settings = new ImageAvailabilityShredSettings();
            IProcessor[] processors = new IProcessor[] {
                new ImageAvailabilityProcedureProcessor(settings),
                new ImageAvailabilityWorkQueueProcessor(settings)
            };

			return processors;
		}
	}
}
