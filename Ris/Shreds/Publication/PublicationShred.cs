using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Server.ShredHost;
using System;

namespace ClearCanvas.Ris.Shreds.Publication
{
	[ExtensionOf(typeof(ShredExtensionPoint))]
	public class PublicationShred : RisShredBase
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

		protected override IList<IProcessor> GetProcessors()
		{
			PublicationProcessor p = new PublicationProcessor(new PublicationShredSettings());
            return new IProcessor[] { p };
        }

	}
}