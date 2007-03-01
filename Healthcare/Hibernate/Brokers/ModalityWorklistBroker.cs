using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Hibernate;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Healthcare.Workflow.Modality;
using ClearCanvas.Enterprise.Hibernate.Hql;
using ClearCanvas.Enterprise;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
    [ExtensionOf(typeof(BrokerExtensionPoint))]
    public class ModalityWorklistBroker : Broker, IModalityWorklistBroker
    {
        public IList<WorklistQueryResult> GetWorklist(ModalityProcedureStepSearchCriteria criteria, string patientProfileAuthority)
        {
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
            query.Selectors.Add(new HqlSelector("State", "sps.State"));

            query.Conditions.AddRange(HqlCondition.FromSearchCriteria("sps", criteria));

            // add some criteria to filter the patient profiles
            PatientProfileSearchCriteria profileCriteria = new PatientProfileSearchCriteria();
            profileCriteria.Mrn.AssigningAuthority.EqualTo(patientProfileAuthority);
            query.Conditions.AddRange(HqlCondition.FromSearchCriteria("pp", profileCriteria));

            List<WorklistQueryResult> items = new List<WorklistQueryResult>();
            foreach (object[] tuple in ExecuteHql(query))
            {
                items.Add( (WorklistQueryResult)Activator.CreateInstance(typeof(WorklistQueryResult), tuple) );
            }
            return items;
        }

        public WorklistQueryResult GetWorklistItem(EntityRef mpsRef, string patientProfileAuthority)
        {
            ModalityProcedureStepSearchCriteria mpsCriteria = new ModalityProcedureStepSearchCriteria(mpsRef);
            IList<WorklistQueryResult> results = this.GetWorklist(mpsCriteria, patientProfileAuthority);

            // expect exactly one result
            return CollectionUtils.FirstElement<WorklistQueryResult>(results);
        }

    }
}
