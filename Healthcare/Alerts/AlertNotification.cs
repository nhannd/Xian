#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;

namespace ClearCanvas.Healthcare.Alerts
{
	public class AlertNotification
	{
		private readonly string _alertId;
		private readonly List<string> _reasons;

		public AlertNotification(string alertId, IEnumerable<string> reasons)
		{
			_alertId = alertId;
			_reasons = new List<string>(reasons);
		}

		public string AlertId
		{
			get { return _alertId; }
		}

		public IEnumerable<string> Reasons
		{
			get { return _reasons; }
		}
	}
}
