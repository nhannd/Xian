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

        public Worklist()
        {
            _requestedProcedureTypeGroups = new HybridSet();

        }
        
        public virtual IList GetWorklist(Staff currentUserStaff, IPersistenceContext context)
        {
            // TODO: Implementation
            return null;
        }

        public virtual int GetWorklistCount(Staff currentUserStaff, IPersistenceContext context)
        {
            // TODO: Implementation
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

        protected static T GetBroker<T>(IPersistenceContext context) where T : IPersistenceBroker
        {
            return context.GetBroker<T>();
        }
    }
}