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
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare.Brokers
{
    public interface IPriorReportBroker : IPersistenceBroker
    {
		/// <summary>
		/// Obtains the set of procedure types that are relevant to the specified procedure type.
		/// </summary>
		/// <param name="procType"></param>
		/// <returns></returns>
    	IList<ProcedureType> GetRelevantProcedureTypes(ProcedureType procType);

        /// <summary>
        /// Obtains the set of prior procedures relevant to the specified report.
        /// </summary>
        /// <param name="report"></param>
        /// <returns></returns>
        IList<Report> GetPriors(Report report);

        /// <summary>
        /// Obtains the set of prior procedures relevant to the specified order.
        /// </summary>
        /// <returns></returns>
        IList<Report> GetPriors(Order order);

        /// <summary>
        /// Obtains a list of prior procedures relevant to the specified procedures.
        /// </summary>
        /// <param name="procedures"></param>
        /// <returns></returns>
        IList<Report> GetPriors(IEnumerable<Procedure> procedures);

        /// <summary>
        /// Obtains a list of all prior procedures for the specified patient.
        /// </summary>
        /// <param name="patient"></param>
        /// <returns></returns>
        IList<Report> GetPriors(Patient patient);
    }
}
