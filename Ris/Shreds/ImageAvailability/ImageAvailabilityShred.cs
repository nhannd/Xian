#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

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
