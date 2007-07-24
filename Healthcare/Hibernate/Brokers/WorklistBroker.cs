using System.Collections;
using ClearCanvas.Enterprise.Hibernate;
using ClearCanvas.Enterprise.Hibernate.Hql;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
    [ClearCanvas.Common.ExtensionOf(typeof(BrokerExtensionPoint))]
    public class WorklistBroker : EntityBroker<Worklist, WorklistSearchCriteria>, IWorklistBroker
    {
        private readonly string _registrationWorklistHql = "from Worklist w"
                                                           + " where w.class = RegistrationScheduledWorklist"
                                                           + " or w.class = RegistrationCheckedInWorklist"
                                                           + " or w.class = RegistrationInProgressWorklist"
                                                           + " or w.class = RegistrationCancelledWorklist"
                                                           + " or w.class = RegistrationCompletedWorklist";

        private readonly string _technologistWorklistHql = "from Worklist w"
                                                           + " where w.class = TechnologistScheduledWorklist"
                                                           + " or w.class = TechnologistCheckedInWorklist"
                                                           + " or w.class = TechnologistInProgressWorklist"
                                                           + " or w.class = TechnologistCancelledWorklist"
                                                           + " or w.class = TechnologistCompletedWorklist";

        private readonly string _reportingWorklistHql = "from Worklist w"
                                                        + " where w.class = ReportingToBeReportedWorklist";

        #region IWorklistBroker Members

        public IList FindAllRegistrationWorklists(Staff currentStaff)
        {
            HqlQuery query = new HqlQuery(_registrationWorklistHql);
            return this.ExecuteHql(query);
        }

        public IList FindAllTechnologistWorklists(Staff currentStaff)
        {
            HqlQuery query = new HqlQuery(_technologistWorklistHql);
            return this.ExecuteHql(query);
        }

        public IList FindAllReportingWorklists(Staff currentStaff)
        {
            HqlQuery query = new HqlQuery(_reportingWorklistHql);
            return this.ExecuteHql(query);
        }

        #endregion
    }
}
