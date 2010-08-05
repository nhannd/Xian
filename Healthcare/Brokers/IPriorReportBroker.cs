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
