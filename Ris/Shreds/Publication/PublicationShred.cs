using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Server.ShredHost;
using System;

namespace ClearCanvas.Ris.Shreds.Publication
{
	[ExtensionOf(typeof(ShredExtensionPoint))]
	public class PublicationShred : RisShredBase
	{
		private readonly PublicationProcessor _processor;

		public PublicationShred()
		{
            PublicationShredSettings settings = new PublicationShredSettings();
			_processor = new PublicationProcessor(settings.BatchSize, TimeSpan.FromSeconds(settings.SleepDurationInSeconds));
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
			List<IProcessor> processors = new List<IProcessor>();
			processors.Add(_processor);
			return processors;
		}

	}
}