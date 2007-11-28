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
        public IList<Report> GetPriorReports(Report report)
        {
            NHibernate.IQuery q = this.Context.GetNamedHqlQuery("relevantPriorsByReport");
            q.SetParameter(0, report);
            return q.List<Report>();
        }

        /// <summary>
        /// Obtains a list of prior reports relevant to the specified procedures.
        /// </summary>
        /// <param name="procedures"></param>
        /// <returns></returns>
        public IList<Report> GetPriorReports(IEnumerable<RequestedProcedure> procedures)
        {
            // because we are using a fixed-form query defined in an external file, 
            // there is no way to query based on all procedures at once, therefore we need one query per procedure
            // this is unfortunate, but hopefully not a major concern in the scheme of things
            // (if so, this method could always be refactored such that the HQL is created dynamically
            // based on the number of procedures)
            HashedSet<Report> reports = new HashedSet<Report>();
            foreach (RequestedProcedure procedure in procedures)
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
        public IList<Report> GetPriorReports(Patient patient)
        {
            NHibernate.IQuery q = this.Context.GetNamedHqlQuery("allPriorsByPatient");
            q.SetParameter(0, patient);
            return q.List<Report>();
        }

        #endregion
    }
}
