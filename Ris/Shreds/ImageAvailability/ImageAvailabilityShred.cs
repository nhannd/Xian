using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Server.ShredHost;

namespace ClearCanvas.Ris.Shreds.ImageAvailability
{
	[ExtensionOf(typeof(ShredExtensionPoint))]
	public class ImageAvailabilityShred : RisShredBase
	{
		private readonly ImageAvailabilityProcedureProcessor _procedureProcessor;
		private readonly ImageAvailabilityWorkQueueItemProcessor _workQueueItemProcessor;

		public ImageAvailabilityShred()
		{
			_procedureProcessor = new ImageAvailabilityProcedureProcessor();
			_workQueueItemProcessor = new ImageAvailabilityWorkQueueItemProcessor();
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
			List<IProcessor> processors = new List<IProcessor>();
			processors.Add(_procedureProcessor);
			processors.Add(_workQueueItemProcessor);
			return processors;
		}
	}
}
