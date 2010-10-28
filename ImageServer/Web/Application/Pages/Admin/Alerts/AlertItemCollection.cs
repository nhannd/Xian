#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Common;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Alerts
{
	/// <summary>
	/// Encapsulates a collection of <see cref="Alert"/> which can be accessed based on the <see cref="ServerEntityKey"/>
	/// </summary>
	public class AlertItemCollection : KeyedCollectionBase<AlertSummary, ServerEntityKey>
	{

		public AlertItemCollection(IList<AlertSummary> list)
			: base(list)
		{
		}

		protected override ServerEntityKey GetKey(AlertSummary item)
		{
			return item.Key;
		}
	}
}
