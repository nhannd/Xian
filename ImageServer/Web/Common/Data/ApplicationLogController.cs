#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Web.Common.Data
{
	public class ApplicationLogController
	{
		#region Private Members

		private readonly ApplicationLogAdaptor _adaptor = new ApplicationLogAdaptor();
		#endregion

		public IList<ApplicationLog> GetRangeLogs(ApplicationLogSelectCriteria criteria, int startIndex, int maxRows)
		{
			return _adaptor.GetRange(criteria, startIndex, maxRows);
		}

		public int GetApplicationLogCount(ApplicationLogSelectCriteria criteria)
		{
			return _adaptor.GetCount(criteria);
		}
	}
}
