using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Enterprise.Hibernate;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Hibernate.Hql;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
    [ExtensionOf(typeof(BrokerExtensionPoint))]
    public class ReportingWorklistBroker : Broker, IReportingWorklistBroker
    {
        public IList<ReportingWorklistQueryResult> GetWorklist(Type stepClass, ReportingProcedureStepSearchCriteria criteria)
        {
            HqlReportQuery query = new HqlReportQuery(new HqlFrom("rps", stepClass.Name));
            query.Joins.Add(new HqlJoin("rp", "rps.RequestedProcedure"));
            query.Joins.Add(new HqlJoin("rpt", "rp.Type"));
            query.Joins.Add(new HqlJoin("o", "rp.Order"));
            query.Joins.Add(new HqlJoin("ds", "o.DiagnosticService"));
            query.Joins.Add(new HqlJoin("p", "o.Patient"));
            query.Joins.Add(new HqlJoin("pp", "p.Profiles"));

            query.Selectors.Add(new HqlSelector("Patient", "p"));
            query.Selectors.Add(new HqlSelector("PatientProfile", "pp"));
            query.Selectors.Add(new HqlSelector("Order", "o"));
            query.Selectors.Add(new HqlSelector("RequestedProcedure", "rp"));
            query.Selectors.Add(new HqlSelector("WorkflowStep", "rps"));
            query.Selectors.Add(new HqlSelector("Mrn", "pp.Mrn"));
            query.Selectors.Add(new HqlSelector("PatientName", "pp.Name"));
            query.Selectors.Add(new HqlSelector("AccessionNumber", "o.AccessionNumber"));
            query.Selectors.Add(new HqlSelector("DiagnosticService", "ds.Name"));
            query.Selectors.Add(new HqlSelector("RequestedProcedureName", "rpt.Name"));
            query.Selectors.Add(new HqlSelector("Priority", "o.Priority"));
            query.Selectors.Add(new HqlSelector("State", "rps.State"));

            query.Conditions.AddRange(HqlCondition.FromSearchCriteria("rps", criteria));



            List<ReportingWorklistQueryResult> items = new List<ReportingWorklistQueryResult>();
            foreach (object[] tuple in ExecuteHql(query))
            {
                items.Add((ReportingWorklistQueryResult)Activator.CreateInstance(typeof(ReportingWorklistQueryResult), tuple));
            }
            return items;
        }
    }
}
