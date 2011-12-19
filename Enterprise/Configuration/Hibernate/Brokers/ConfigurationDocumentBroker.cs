#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca

// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Hibernate.Hql;

namespace ClearCanvas.Enterprise.Configuration.Hibernate.Brokers
{
	partial class ConfigurationDocumentBroker
	{
		#region Implementation of IConfigurationDocumentBroker

		public IList<ConfigurationDocument> Find(ConfigurationDocumentSearchCriteria documentCriteria, ConfigurationDocumentBodySearchCriteria bodyCriteria, SearchResultPage page)
		{
			var hqlFrom = new HqlFrom(typeof(ConfigurationDocument).Name, "doc");
			hqlFrom.Joins.Add(new HqlJoin("doc.Body", "body"));

			var query = new HqlProjectionQuery(hqlFrom);
			query.Selects.Add(new HqlSelect("doc"));
			query.Conditions.AddRange(HqlCondition.FromSearchCriteria("doc", documentCriteria));
			query.Conditions.AddRange(HqlCondition.FromSearchCriteria("body", bodyCriteria));
			query.Page = page;

			return ExecuteHql<ConfigurationDocument>(query);
		}

		#endregion
	}
}
