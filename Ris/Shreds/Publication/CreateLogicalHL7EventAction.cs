#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

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

		public void Execute(ReportPart reportPart, IPersistenceContext context)
		{
			if (_enabled == false)
				return;

			LogicalHL7Event.ReportPublished.EnqueueEvents(reportPart.Report);
		}
	}
}
