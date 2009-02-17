using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Shreds;

namespace ClearCanvas.Ris.Shreds.Publication
{
	[ExtensionOf(typeof(ShredExtensionPoint))]
	public class PublicationShred : QueueProcessorShred
	{
		public PublicationShred()
		{
		}

		public override string GetDisplayName()
		{
			return SR.PublicationShredName;
		}

		public override string GetDescription()
		{
			return SR.PublicationShredDescription;
		}

		protected override IList<QueueProcessor> GetProcessors()
		{
			PublicationProcessor p = new PublicationProcessor(new PublicationShredSettings());
			return new QueueProcessor[] { p };
        }

	}
}