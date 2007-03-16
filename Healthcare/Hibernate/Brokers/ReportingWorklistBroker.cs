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

            query.Selects.Add(new HqlSelect("p"));
            query.Selects.Add(new HqlSelect("pp"));
            query.Selects.Add(new HqlSelect("o"));
            query.Selects.Add(new HqlSelect("rp"));
            query.Selects.Add(new HqlSelect("rps"));
            query.Selects.Add(new HqlSelect("pp.Mrn"));
            query.Selects.Add(new HqlSelect("pp.Name"));
            query.Selects.Add(new HqlSelect("o.AccessionNumber"));
            query.Selects.Add(new HqlSelect("ds.Name"));
            query.Selects.Add(new HqlSelect("rpt.Name"));
            query.Selects.Add(new HqlSelect("o.Priority"));
            query.Selects.Add(new HqlSelect("rps.State"));

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
