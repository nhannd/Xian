using System;
using System.Collections;
using System.Collections.Generic;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Hibernate;
using ClearCanvas.Enterprise.Hibernate.Hql;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Healthcare.Workflow.Modality;

using NHibernate;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
    [ExtensionOf(typeof(BrokerExtensionPoint))]
    public class ModalityWorklistBroker : Broker, IModalityWorklistBroker
    {
        public IList<WorklistItem> GetWorklist(ModalityProcedureStepSearchCriteria criteria, string patientProfileAuthority)
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

            query.Selects.Add(new HqlSelect("p"));
            query.Selects.Add(new HqlSelect("pp"));
            query.Selects.Add(new HqlSelect("o"));
            query.Selects.Add(new HqlSelect("rp"));
            query.Selects.Add(new HqlSelect("sps"));
            query.Selects.Add(new HqlSelect("pp.Mrn"));
            query.Selects.Add(new HqlSelect("pp.Name"));
            query.Selects.Add(new HqlSelect("v.VisitNumber"));
            query.Selects.Add(new HqlSelect("o.AccessionNumber"));
            query.Selects.Add(new HqlSelect("ds.Name"));
            query.Selects.Add(new HqlSelect("rpt.Name"));
            query.Selects.Add(new HqlSelect("spst.Name"));
            query.Selects.Add(new HqlSelect("m.Name"));
            query.Selects.Add(new HqlSelect("o.Priority"));
            query.Selects.Add(new HqlSelect("sps.State"));

            query.Conditions.AddRange(HqlCondition.FromSearchCriteria("sps", criteria));

            // add some criteria to filter the patient profiles
            PatientProfileSearchCriteria profileCriteria = new PatientProfileSearchCriteria();
            profileCriteria.Mrn.AssigningAuthority.EqualTo(patientProfileAuthority);
            query.Conditions.AddRange(HqlCondition.FromSearchCriteria("pp", profileCriteria));

            List<WorklistItem> items = new List<WorklistItem>();
            foreach (object[] tuple in ExecuteHql(query))
            {
                items.Add((WorklistItem)Activator.CreateInstance(typeof(WorklistItem), tuple));
            }
            return items;
        }

        public WorklistItem GetWorklistItem(EntityRef mpsRef, string patientProfileAuthority)
        {
            ModalityProcedureStepSearchCriteria mpsCriteria = new ModalityProcedureStepSearchCriteria(mpsRef);
            IList<WorklistItem> results = this.GetWorklist(mpsCriteria, patientProfileAuthority);

            // expect exactly one result
            return CollectionUtils.FirstElement<WorklistItem>(results);
        }

        #region IModalityWorklistBroker Members

        public IList<WorklistItem> GetWorklist(string worklistClassName)
        {
            string defaultHqlQuery = 
                "select mps, pp.Mrn, pp.Name, o.AccessionNumber, o.Priority, rpt, spst, m" +
                " from ModalityProcedureStep mps" +
                " join mps.Type spst" +
                " join mps.Modality m" +
                " join mps.RequestedProcedure rp" +
                " join rp.Type rpt" +
                " join rp.Order o" +
                " join o.DiagnosticService ds" +
                " join o.Visit v" +
                " join o.Patient p" +
                " join p.Profiles pp" +
                " where mps.State = :mpsState";
            //" and mps.Scheduling.StartTime between :mpsSchedulingStartTimeBegin and :mpsSchedulingStartTimeEnd";

            string hqlQuery = String.Concat(defaultHqlQuery, GetSubQuery(worklistClassName));
            IQuery query = this.Context.CreateHibernateQuery(hqlQuery);
            SetNamedParameters(query, worklistClassName);

            List<WorklistItem> results = new List<WorklistItem>();
            foreach (object[] tuple in query.List())
            {
                WorklistItem item = (WorklistItem)Activator.CreateInstance(typeof(WorklistItem), tuple);
                results.Add(item);
            }
            return results;
        }

        public int GetWorklistCount(string worklistClassName)
        {
            string defaultHqlQuery = 
                            "select count(*) " + 
                            " from ModalityProcedureStep mps" +
                            " join mps.Type spst" +
                            " join mps.Modality m" +
                            " join mps.RequestedProcedure rp" +
                            " join rp.Type rpt" +
                            " join rp.Order o" +
                            " join o.DiagnosticService ds" +
                            " join o.Visit v" +
                            " join o.Patient p" +
                            " join p.Profiles pp" +
                            " where mps.State = :mpsState";
            //" and mps.Scheduling.StartTime between :mpsSchedulingStartTimeBegin and :mpsSchedulingStartTimeEnd";

            string hqlQuery = String.Concat(defaultHqlQuery, GetSubQuery(worklistClassName));
            IQuery query = this.Context.CreateHibernateQuery(hqlQuery);
            SetNamedParameters(query, worklistClassName);

            IList list = query.List();
            return (int)list[0];
        }

        public WorklistItem GetWorklistItem(string worklistClassName)
        {
            throw new NotImplementedException();
        }

        #endregion

        private void SetNamedParameters(IQuery query, string worklistClassName)
        {
            //query.SetParameter("mpsSchedulingStartTimeBegin", Platform.Time.Date.ToString());
            //query.SetParameter("mpsSchedulingStartTimeEnd", Platform.Time.Date.AddDays(1).ToString());

            switch (worklistClassName)
            {
                case "ClearCanvas.Healthcare.Workflow.Modality.Worklists+Scheduled":
                case "ClearCanvas.Healthcare.Workflow.Modality.Worklists+CheckedIn":
                    query.SetParameter("mpsState", "SC");
                    //query.SetParameter("cpsSchedulingStartTimeBegin", Platform.Time.Date.ToString());
                    //query.SetParameter("cpsSchedulingStartTimeEnd", Platform.Time.Date.AddDays(1).ToString());
                    break;
                case "ClearCanvas.Healthcare.Workflow.Modality.Worklists+InProgress":
                    query.SetParameter("mpsState", "IP");
                    break;
                case "ClearCanvas.Healthcare.Workflow.Modality.Worklists+Suspended":
                    query.SetParameter("mpsState", "SU");
                    break;
                case "ClearCanvas.Healthcare.Workflow.Modality.Worklists+Completed":
                    query.SetParameter("mpsState", "CM");
                    break;
                case "ClearCanvas.Healthcare.Workflow.Modality.Worklists+Cancelled":
                    query.SetParameter("mpsState", "DC");
                    break;
                default:
                    break;
            }
        }

        private string GetSubQuery(string worklistClassName)
        {
            string subQuery = null;
            switch (worklistClassName)
            {
                case "ClearCanvas.Healthcare.Workflow.Modality.Worklists+Scheduled":
                    subQuery = " and rp not in (select rp from CheckInProcedureStep cps join cps.RequestedProcedure rp" +
                        //" where (cps.Scheduling.StartTime between :cpsSchedulingStartTimeBegin and :cpsSchedulingStartTimeEnd)";
                                ")";
                    break;
                case "ClearCanvas.Healthcare.Workflow.Modality.Worklists+CheckedIn":
                    subQuery = " and rp in (select rp from CheckInProcedureStep cps join cps.RequestedProcedure rp" +
                        //" where (cps.Scheduling.StartTime between :cpsSchedulingStartTimeBegin and :cpsSchedulingStartTimeEnd)";
                                ")";
                    break;
                case "ClearCanvas.Healthcare.Workflow.Modality.Worklists+Suspended":
                case "ClearCanvas.Healthcare.Workflow.Modality.Worklists+InProgress":
                case "ClearCanvas.Healthcare.Workflow.Modality.Worklists+Completed":
                case "ClearCanvas.Healthcare.Workflow.Modality.Worklists+Cancelled":
                default:
                    break;
            }

            return subQuery;
        }
    }
}
