#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Hibernate;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
	[ExtensionOf(typeof(BrokerExtensionPoint))]
	public class PatientHistoryBroker : Broker, IPatientHistoryBroker
	{
		/// <summary>
		/// Obtains the set of all orders for the specified patient.
		/// </summary>
		/// <param name="patient"></param>
		/// <returns></returns>
		public IList<Order> GetOrderHistory(Patient patient)
		{
			var namedHqlQuery = this.GetNamedHqlQuery("orderHistory");
			namedHqlQuery.SetParameter(0, patient);

			// uniquefy the results in case fetch joins added additional lines
			return CollectionUtils.Unique(namedHqlQuery.List<Order>());
		}

		/// <summary>
		/// Obtains the set of all procedures for the specified patient.
		/// </summary>
		/// <param name="patient"></param>
		/// <returns></returns>
		public IList<Procedure> GetProcedureHistory(Patient patient)
		{
			var namedHqlQuery = this.GetNamedHqlQuery("procedureHistory");
			namedHqlQuery.SetParameter(0, patient);

			var procedures = CollectionUtils.Map<object[], Procedure>(namedHqlQuery.List(), tuple => (Procedure)tuple[0]);

			// uniquefy the results in case fetch joins added additional lines)
			return CollectionUtils.Unique(procedures);
		}

		/// <summary>
		/// Obtains the set of all reports for the specified patient.
		/// </summary>
		/// <param name="patient"></param>
		/// <returns></returns>
		public IList<Report> GetReportHistory(Patient patient)
		{
			var namedHqlQuery = this.GetNamedHqlQuery("reportHistory");
			namedHqlQuery.SetParameter(0, patient);

			// uniquefy the results in case fetch joins added additional lines
			return CollectionUtils.Unique(namedHqlQuery.List<Report>());
		}

		/// <summary>
		/// Obtains the set of all reports for the specified order.
		/// </summary>
		/// <param name="order"></param>
		/// <returns></returns>
		public IList<Report> GetReportsForOrder(Order order)
		{
			var namedHqlQuery = this.GetNamedHqlQuery("reportsForOrder");
			namedHqlQuery.SetParameter(0, order);

			// uniquefy the results in case fetch joins added additional lines
			return CollectionUtils.Unique(namedHqlQuery.List<Report>());
		}
	}

}
