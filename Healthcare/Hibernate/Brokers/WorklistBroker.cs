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
    public class WorklistBroker : Broker, IWorklistBroker
    {
        public IList<WorklistItem> GetWorklist()
        {
            // build query from multiple criteria
            HqlQuery query = new HqlQuery(
                "select pp.Mrn, pp.Name, v.VisitNumber, o.AccessionNumber, ds.Name, rpt.Name, spst.Name, m.Name, o.Priority"
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

            List<WorklistItem> items = new List<WorklistItem>();
            foreach (object[] tuple in ExecuteHql(query))
            {
                items.Add(
                    new WorklistItem(
                        (CompositeIdentifier)tuple[0],
                        (PersonName)tuple[1],
                        (CompositeIdentifier)tuple[2],
                        (string)tuple[3],
                        (string)tuple[4],
                        (string)tuple[5],
                        (string)tuple[6],
                        (string)tuple[7],
                        (OrderPriority)tuple[8]
                    ));
            }

            return items;
        }
    }
}
