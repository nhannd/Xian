using System.Collections;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Authentication;
using ClearCanvas.Enterprise.Hibernate;
using ClearCanvas.Healthcare.Brokers;
using NHibernate;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
    [ExtensionOf(typeof(BrokerExtensionPoint))]
    public class WorklistBroker : EntityBroker<Worklist, WorklistSearchCriteria>, IWorklistBroker
    {
        #region Private Fields

        private readonly string _selectHql = "select w from Worklist w join w.Users u"
                                             + " where u = :user ";

        private readonly string _registrationWorklistHql = " and (w.class = RegistrationScheduledWorklist"
                                                           + " or w.class = RegistrationCheckedInWorklist"
                                                           + " or w.class = RegistrationInProgressWorklist"
                                                           + " or w.class = RegistrationCancelledWorklist"
                                                           + " or w.class = RegistrationCompletedWorklist)";

        private readonly string _technologistWorklistHql = " and (w.class = TechnologistScheduledWorklist"
                                                           + " or w.class = TechnologistCheckedInWorklist"
                                                           + " or w.class = TechnologistInProgressWorklist"
                                                           + " or w.class = TechnologistCancelledWorklist"
                                                           + " or w.class = TechnologistCompletedWorklist)";

        private readonly string _reportingWorklistHql = " and (w.class = ReportingToBeReportedWorklist)";

        #endregion

        #region IWorklistBroker Members

        public IList FindAllRegistrationWorklists(User currentUser)
        {
            return DoQuery(currentUser, _registrationWorklistHql);
        }

        public IList FindAllTechnologistWorklists(User currentUser)
        {
            return DoQuery(currentUser, _technologistWorklistHql);
        }

        public IList FindAllReportingWorklists(User currentUser)
        {
            return DoQuery(currentUser, _reportingWorklistHql);
        }

        public bool NameExistsForType(string name, string type)
        {
            WorklistSearchCriteria criteria = new WorklistSearchCriteria();
            criteria.Name.EqualTo(name);
            return this.Find(criteria).Count != 0;
        }

        #endregion

        #region Private Methods

        private IList DoQuery(User currentUser, string subCriteria)
        {
            IQuery query = this.Context.CreateHibernateQuery(_selectHql + subCriteria);
            query.SetParameter("user", currentUser);
            return query.List();
        }

        #endregion
    }
}
