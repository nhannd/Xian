#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca

// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Enterprise.Configuration.Brokers
{
	partial interface IConfigurationDocumentBroker
	{
		IList<ConfigurationDocument> Find(ConfigurationDocumentSearchCriteria documentCriteria,
										  ConfigurationDocumentBodySearchCriteria bodyCriteria, SearchResultPage page);
	}
}
