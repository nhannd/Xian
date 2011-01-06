#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.Workflow.Reporting;

namespace ClearCanvas.Healthcare.Brokers
{
    public interface IPatientHistoryBroker : IPersistenceBroker
    {
		/// <summary>
		/// Obtains the set of all orders for the specified patient.
		/// </summary>
		/// <param name="patient"></param>
		/// <returns></returns>
        IList<Order> GetOrderHistory(Patient patient);

		/// <summary>
		/// Obtains the set of all procedures for the specified patient.
		/// </summary>
		/// <param name="patient"></param>
		/// <returns></returns>
        IList<Procedure> GetProcedureHistory(Patient patient);

		/// <summary>
		/// Obtains the set of all reports for the specified patient.
		/// </summary>
		/// <param name="patient"></param>
		/// <returns></returns>
		IList<Report> GetReportHistory(Patient patient);

    	/// <summary>
    	/// Obtains the set of all reports for the specified order.
    	/// </summary>
    	/// <param name="order"></param>
    	/// <returns></returns>
    	IList<Report> GetReportsForOrder(Order order);
    }
}
