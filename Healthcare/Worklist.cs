using System.Collections;
using ClearCanvas.Enterprise.Core;
using Iesi.Collections;

namespace ClearCanvas.Healthcare
{
    public abstract class Worklist : Entity, IWorklist
    {
        private string _name;
        private string _description;
        private ISet _requestedProcedureTypeGroups;
        private ISet _users;

        public Worklist()
        {
            _requestedProcedureTypeGroups = new HybridSet();
            _users = new HybridSet();
        }
        
        public virtual IList GetWorklist(Staff currentUserStaff, IPersistenceContext context)
        {
            return null;
        }

        public virtual int GetWorklistCount(Staff currentUserStaff, IPersistenceContext context)
        {
            return 0;
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public ISet RequestedProcedureTypeGroups
        {
            get { return _requestedProcedureTypeGroups; }
        }

        public ISet Users
        {
            get { return _users; }
        }

        protected static T GetBroker<T>(IPersistenceContext context) where T : IPersistenceBroker
        {
            return context.GetBroker<T>();
        }
    }
}