using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare.Brokers
{
    public interface IPriorReportBroker : IPersistenceBroker
    {
        /// <summary>
        /// Obtains a list of prior reports relevant to the specified report.
        /// </summary>
        /// <param name="report"></param>
        /// <returns></returns>
        IList<Report> GetPriorReports(Report report);

        /// <summary>
        /// Obtains a list of prior reports relevant to the specified procedures.
        /// </summary>
        /// <param name="procedures"></param>
        /// <returns></returns>
        IList<Report> GetPriorReports(IEnumerable<RequestedProcedure> procedures);

        /// <summary>
        /// Obtains a list of all prior reports for the specified patient.
        /// </summary>
        /// <param name="patient"></param>
        /// <returns></returns>
        IList<Report> GetPriorReports(Patient patient);
    }
}
