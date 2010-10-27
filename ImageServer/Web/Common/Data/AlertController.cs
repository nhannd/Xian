#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Web.Common.Data
{
	public class AlertController
	{
        private readonly AlertAdaptor _adaptor = new AlertAdaptor();

        public IList<Alert> GetAllAlerts()
        {
            AlertSelectCriteria searchCriteria = new AlertSelectCriteria();
            searchCriteria.InsertTime.SortAsc(0);
            return GetAlerts(searchCriteria);
        }

        public IList<Alert> GetAlerts(AlertSelectCriteria criteria)
        {
            return _adaptor.Get(criteria);
        }

        public IList<Alert> GetRangeAlerts(AlertSelectCriteria criteria, int startIndex, int maxRows)
        {
            return _adaptor.GetRange(criteria, startIndex, maxRows);
        }

        public int GetAlertsCount(AlertSelectCriteria criteria)
        {
            return _adaptor.GetCount(criteria);
        }

        public bool DeleteAlert(Alert item)
        {
            return _adaptor.Delete(item.Key);
        }

        public bool DeleteAlertItems(IList<Alert> items)
        {
            foreach (Alert item in items)
            {
                if (_adaptor.Delete(item.Key) == false)
                    return false;
            }

            return true;
        }

        public bool DeleteAlertItem(ServerEntityKey key)
        {
            return _adaptor.Delete(key);   
        }

        public bool DeleteAllAlerts()
        {
            return _adaptor.Delete(new AlertSelectCriteria());
        }
	}
}
