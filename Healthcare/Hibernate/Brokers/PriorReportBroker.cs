#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Hibernate;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Hibernate.Hql;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
    /// <summary>
    /// Implementation of <see cref="IPriorReportBroker"/>. See PriorReportBroker.hbm.xml for queries.
    /// </summary>
    [ExtensionOf(typeof(BrokerExtensionPoint))]
    public class PriorReportBroker : Broker, IPriorReportBroker
    {
        #region IPriorReportBroker Members

    	/// <summary>
    	/// Obtains the set of procedure types that are relevant to the specified procedure type.
    	/// </summary>
    	/// <param name="procType"></param>
    	/// <returns></returns>
    	public IList<ProcedureType> GetRelevantProcedureTypes(ProcedureType procType)
		{
			// by caching this query, we effectively build up a kind of in-memory index
			// of relevant procedure types
			// TODO: we could even set up a different cache-region with a much longer expiry time (eg hours) if we need to make this fast!!!
			NHibernate.IQuery q = this.Context.GetNamedHqlQuery("relevantProcedureTypes");
			q.SetCacheable(true);
			q.SetParameter(0, procType);
			return q.List<ProcedureType>();
		}

    	/// <summary>
    	/// Obtains the set of prior procedures relevant to the specified report.
    	/// </summary>
    	/// <param name="report"></param>
    	/// <returns></returns>
    	public IList<Report> GetPriors(Report report)
        {
        	return GetPriorsHelper(report.Procedures);
        }

    	/// <summary>
    	/// Obtains the set of prior procedures relevant to the specified order.
    	/// </summary>
    	/// <returns></returns>
    	public IList<Report> GetPriors(Order order)
        {
        	return GetPriorsHelper(order.Procedures);
        }

    	/// <summary>
    	/// Obtains a list of prior procedures relevant to the specified procedures.
    	/// </summary>
    	/// <param name="procedures"></param>
    	/// <returns></returns>
    	public IList<Report> GetPriors(IEnumerable<Procedure> procedures)
        {
        	return GetPriorsHelper(procedures);
        }

    	/// <summary>
    	/// Obtains a list of all prior procedures for the specified patient.
    	/// </summary>
    	/// <param name="patient"></param>
    	/// <returns></returns>
    	public IList<Report> GetPriors(Patient patient)
        {
            NHibernate.IQuery q = this.Context.GetNamedHqlQuery("allPriorsByPatient");
            q.SetParameter(0, patient);
            return q.List<Report>();
        }

        #endregion

		/// <summary>
		/// Obtains the set of prior reports relevant to the specified set of procedures, which must all be for the same patient.
		/// </summary>
		/// <remarks>
		/// Excludes draft and deleted reports, as well as any report that is directly associated with the input procedures.
		/// </remarks>
		/// <param name="inputProcedures"></param>
		/// <returns></returns>
		private IList<Report> GetPriorsHelper(IEnumerable<Procedure> inputProcedures)
		{
			// Notes on strategy:
			// The algorithm is split into two parts.
			
			// 1) Obtain the set of relevant procedure types, based on the set of input procedures.
			// This query is for practical purposes a constant function, because the relevance groups
			// rarely change.  Therefore, the query can be cached, which should improve performance
			// (even though the raw performance seems quite reasonable to begin with).
			List<ProcedureType> inputTypes = CollectionUtils.Map<Procedure, ProcedureType>(inputProcedures,
				delegate(Procedure p) { return p.Type; });

			List<ProcedureType> relevantTypes = new List<ProcedureType>();
			foreach (ProcedureType type in inputTypes)
			{
				// get relevant types for each input procedure
				// typically there should be 1 to 3 elements in this collection, with 1 being the most common scenario
				// therefore, don't worry about having to do a separate query for each
				// (we could try to write a query that would accept all inputs at once, but this would defeat the value of
				// caching the query)
				relevantTypes.AddRange(GetRelevantProcedureTypes(type));
			}

			// 2) Obtain the set of prior reports for this patient, where the report is associated
			// with procedure types as determined above

			// obtain patient from any of the input procedures (since all must be for same patient)
			Patient patient = CollectionUtils.FirstElement(inputProcedures).Order.Patient;


			// build query for prior reports
			HqlProjectionQuery reportsQuery = new HqlProjectionQuery(
				new HqlFrom("Report", "priorReport", new HqlJoin[]
        		                                     	{
        		                                     		new HqlJoin("priorReport.Procedures", "priorProcedure"),
        		                                     		new HqlJoin("priorProcedure.Type", "priorProcedureType"),

															// add fetch join for procedures, to save one query
															new HqlJoin("priorReport.Procedures", null, HqlJoinMode.Inner, true)
        		                                     	}),
				new HqlSelect[] { new HqlSelect("priorReport") });

			// must be for same patient
			reportsQuery.Conditions.Add(HqlCondition.EqualTo("priorProcedure.Order.Patient", patient));

			// exclude the input procedures
			reportsQuery.Conditions.Add(HqlCondition.NotIn("priorProcedure", inputProcedures));

			// not a draft or deleted report
			reportsQuery.Conditions.Add(HqlCondition.In("priorReport.Status", ReportStatus.P, ReportStatus.F, ReportStatus.C));

			// consider only reports with relevant procedure types
			if (relevantTypes.Count > 0)
				reportsQuery.Conditions.Add(HqlCondition.In("priorProcedureType", relevantTypes));

			IList<Report> reports = ExecuteHql<Report>(reportsQuery);

			// ensure unique results are returned (because fetch joins may have introduced duplicates into result set)
			return CollectionUtils.Unique(reports);
		}
	}
}
