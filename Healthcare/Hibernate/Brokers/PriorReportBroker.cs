#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using Iesi.Collections.Generic;

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
        /// Obtains a list of prior reports for the specified report, optionally constrained by Relevance grouping.
        /// </summary>
        /// <param name="report"></param>
        /// <returns></returns>
        public IList<Report> GetPriors(Report report)
        {
            NHibernate.IQuery q = this.Context.GetNamedHqlQuery("relevantPriorsByReport");
            q.SetParameter(0, report);
            return q.List<Report>();
        }

        public IList<Report> GetPriors(Order order)
        {
            NHibernate.IQuery q = this.Context.GetNamedHqlQuery("relevantPriorsByOrder");
            q.SetParameter(0, order);
            return q.List<Report>();
        }

        /// <summary>
        /// Obtains a list of prior reports relevant to the specified procedures.
        /// </summary>
        /// <param name="procedures"></param>
        /// <returns></returns>
        public IList<Report> GetPriors(IEnumerable<Procedure> procedures)
        {
            // because we are using a fixed-form query defined in an external file, 
            // there is no way to query based on all procedures at once, therefore we need one query per procedure
            // this is unfortunate, but hopefully not a major concern in the scheme of things
            // (if so, this method could always be refactored such that the HQL is created dynamically
            // based on the number of procedures)
            HashedSet<Report> reports = new HashedSet<Report>();
            foreach (Procedure procedure in procedures)
            {
                NHibernate.IQuery q = this.Context.GetNamedHqlQuery("relevantPriorsByProcedure");
                q.SetParameter(0, procedure);
                reports.AddAll(q.List<Report>());
            }
            return new List<Report>(reports);
        }

        /// <summary>
        /// Obtains a list of all prior reports for the specified patient.
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
    }
}
