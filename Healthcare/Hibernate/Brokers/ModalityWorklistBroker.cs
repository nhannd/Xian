using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Hibernate;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Enterprise.Hibernate.Hql;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
    [ExtensionOf(typeof(BrokerExtensionPoint))]
    public class ModalityWorklistBroker : Broker, IModalityWorklistBroker
    {
        public IList<ModalityWorklistQueryResult> GetWorklist(ModalityProcedureStepSearchCriteria criteria)
        {
/*            
            HqlQuery query = new HqlQuery(
                "select new WorklistItem(pp.Mrn, pp.Name, v.VisitNumber, o.AccessionNumber, ds.Name, rpt.Name, spst.Name, m.Name, o.Priority)"
                + " from ScheduledProcedureStep sps"
                + " join sps.Type spst"
                + " join sps.Modality m"
                + " join sps.RequestedProcedure rp"
                + " join rp.Type rpt"
                + " join rp.Order o"
                + " join o.DiagnosticService ds"
                + " join o.Visit v"
                + " join o.Patient p"
                + " join p.Profiles pp"
            );
*/            

            HqlReportQuery query = new HqlReportQuery(new HqlFrom("sps", "ModalityProcedureStep"));
            query.Joins.Add(new HqlJoin("spst", "sps.Type"));
            query.Joins.Add(new HqlJoin("m", "sps.Modality"));
            query.Joins.Add(new HqlJoin("rp", "sps.RequestedProcedure"));
            query.Joins.Add(new HqlJoin("rpt", "rp.Type"));
            query.Joins.Add(new HqlJoin("o", "rp.Order"));
            query.Joins.Add(new HqlJoin("ds", "o.DiagnosticService"));
            query.Joins.Add(new HqlJoin("v", "o.Visit"));
            query.Joins.Add(new HqlJoin("p", "o.Patient"));
            query.Joins.Add(new HqlJoin("pp", "p.Profiles"));

            query.Selectors.Add(new HqlSelector("Patient", "p"));
            query.Selectors.Add(new HqlSelector("PatientProfile", "pp"));
            query.Selectors.Add(new HqlSelector("Order", "o"));
            query.Selectors.Add(new HqlSelector("RequestedProcedure", "rp"));
            query.Selectors.Add(new HqlSelector("WorkflowStep", "sps"));
            query.Selectors.Add(new HqlSelector("Mrn", "pp.Mrn"));
            query.Selectors.Add(new HqlSelector("PatientName", "pp.Name"));
            query.Selectors.Add(new HqlSelector("VisitNumber", "v.VisitNumber"));
            query.Selectors.Add(new HqlSelector("AccessionNumber", "o.AccessionNumber"));
            query.Selectors.Add(new HqlSelector("DiagnosticService", "ds.Name"));
            query.Selectors.Add(new HqlSelector("Procedure", "rpt.Name"));
            query.Selectors.Add(new HqlSelector("ScheduledStep", "spst.Name"));
            query.Selectors.Add(new HqlSelector("Modality", "m.Name"));
            query.Selectors.Add(new HqlSelector("Priority", "o.Priority"));
            query.Selectors.Add(new HqlSelector("Status", "sps.Status"));

            query.Conditions.AddRange(HqlCondition.FromSearchCriteria("sps", criteria));
            


            List<ModalityWorklistQueryResult> items = new List<ModalityWorklistQueryResult>();
            foreach (object[] tuple in ExecuteHql(query))
            {
                items.Add( (ModalityWorklistQueryResult)Activator.CreateInstance(typeof(ModalityWorklistQueryResult), tuple) );
            }
            return items;
        }
    }
}
