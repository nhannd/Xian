#region License

// Copyright (c) 2010, ClearCanvas Inc.
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

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Hibernate;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Common.Utilities;

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
			return GetRelevantTypes(procType);
		}

		/// <summary>
		/// Obtains a list of priors for the patient associated with the specified report,
		/// optionally filtering by relevancy to the specified report.
		/// </summary>
		/// <returns></returns>
		public IList<Prior> GetPriors(Report report, bool relevantOnly)
		{
			var order = CollectionUtils.FirstElement(report.Procedures).Order;
			return GetPriorsHelper(order.Patient, report.Procedures, report.Procedures, order.Procedures, relevantOnly);
		}


		/// <summary>
		/// Obtains a list of priors for the patient associated with the specified order,
		/// optionally filtering by relevancy to the specified order.
		/// </summary>
		/// <returns></returns>
		public IList<Prior> GetPriors(Order order, bool relevantOnly)
		{
			return GetPriorsHelper(order.Patient, order.Procedures, new Procedure[]{}, order.Procedures, relevantOnly);
		}

		#endregion

		/// <summary>
		/// 
		/// </summary>
		/// <param name="patient"></param>
		/// <param name="relevancyReferenceProcedures">Set of procedures whose types are used to determine relevancy.</param>
		/// <param name="excludeProcedures">Set of procedures to exclude, even if they are relevant.</param>
		/// <param name="includeProcedures">Set of procedure to include, even if they are not relevant. These must
		/// not be in the exclude list.</param>
		/// <param name="relevantOnly">Specifies whether to apply the relevancy filter.</param>
		/// <returns></returns>
		private IList<Prior> GetPriorsHelper(
			Patient patient,
			ICollection<Procedure> relevancyReferenceProcedures,
			ICollection<Procedure> excludeProcedures,
			ICollection<Procedure> includeProcedures,
			bool relevantOnly)
		{
			var priors = GetAllPriorsForPatient(patient);

			// filter out priors representing the set of excluded procedures
			priors = CollectionUtils.Select(priors, prior => !excludeProcedures.Contains(prior.Procedure));

			if (relevantOnly)
			{
				var relevantTypes = GetRelevantTypes(relevancyReferenceProcedures);

				// select only those rows where
				// the procedure type is relevant as determined above, or the procedure is in the includeProcedures list
				priors = CollectionUtils.Select(priors, prior => relevantTypes.Contains(prior.ProcedureType) || includeProcedures.Contains(prior.Procedure));
			}

			return priors;
		}

		private IList<ProcedureType> GetRelevantTypes(ProcedureType procType)
		{
			var q = this.GetNamedHqlQuery("relevantProcedureTypes");
			q.SetCacheable(true);
			q.SetParameter(0, procType);
			return q.List<ProcedureType>();
		}

		private IList<ProcedureType> GetRelevantTypes(ICollection<Procedure> inputProcedures)
		{
			var procTypes = CollectionUtils.Map<Procedure, ProcedureType>(inputProcedures, p => p.Type);
			var relevantTypes = new List<ProcedureType>();
			foreach (var type in procTypes)
			{
				// get relevant types for each input procedure type
				// typically there should be 1 to 3 elements in this collection, with 1 being the most common scenario
				// therefore, don't worry about having to do a separate query for each
				// (we could try to write a query that would accept all inputs at once, but this would defeat the value of
				// caching the query)
				relevantTypes.AddRange(GetRelevantTypes(type));
			}
			return CollectionUtils.Unique(relevantTypes);
		}

		private IList<Prior> GetAllPriorsForPatient(Patient patient)
		{
			var q = this.GetNamedHqlQuery("allPriorsByPatient");
			q.SetParameter(0, patient);
			return CollectionUtils.Map(ExecuteHql<object[]>(q),
									   (object[] tuple) => new Prior((Report)tuple[0], (Procedure)tuple[1], (ProcedureType)tuple[2], (Order)tuple[3]));
		}
	}
}
