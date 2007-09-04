using System.Collections;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Authentication;
using ClearCanvas.Enterprise.Hibernate;
using ClearCanvas.Healthcare.Brokers;
using NHibernate;
using ClearCanvas.Common.Utilities;

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

        public Worklist FindWorklist(string name, string type)
        {
            IQuery query = this.Context.CreateHibernateQuery("select w from Worklist w where w.Name = :name and w.class = " + type);
            query.SetParameter("name", name);
            return CollectionUtils.FirstElement<Worklist>(query.List());
        }

        public bool NameExistsForType(string name, string type)
        {
            IQuery query = this.Context.CreateHibernateQuery("select w from Worklist w where w.Name = :name and w.class = " + type);
            query.SetParameter("name", name);
            return query.List().Count > 0;
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
