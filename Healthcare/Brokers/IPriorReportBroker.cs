#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare.Brokers
{
	/// <summary>
	/// Represents a "prior", which is essentially a tuple consisting of a procedure, its associated order and report.
	/// </summary>
	public class Prior
	{
		public Prior(Report r, Procedure rp, ProcedureType pt, Order o)
		{
			this.Report = r;
			this.Procedure = rp;
			this.ProcedureType = pt;
			this.Order = o;
		}

		public Report Report { get; private set; }
		public Procedure Procedure { get; private set; }
		public ProcedureType ProcedureType { get; private set; }
		public Order Order { get; private set; }
	}

	public interface IPriorReportBroker : IPersistenceBroker
	{
		/// <summary>
		/// Obtains the set of procedure types that are relevant to the specified procedure type.
		/// </summary>
		/// <param name="procType"></param>
		/// <returns></returns>
		IList<ProcedureType> GetRelevantProcedureTypes(ProcedureType procType);

		/// <summary>
		/// Obtains a list of priors for the patient associated with the specified report,
		/// optionally filtering by relevancy to the specified report.
		/// </summary>
		/// <returns></returns>
		IList<Prior> GetPriors(Report report, bool relevantOnly);

		/// <summary>
		/// Obtains a list of priors for the patient associated with the specified order,
		/// optionally filtering by relevancy to the specified order.
		/// </summary>
		/// <returns></returns>
		IList<Prior> GetPriors(Order order, bool relevantOnly);
	}
}
