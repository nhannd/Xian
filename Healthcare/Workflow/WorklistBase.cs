using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare.Workflow
{
    public abstract class WorklistItemBase : IWorklistItem
    {
        private IWorklistItemKey _key;

        protected WorklistItemBase(IWorklistItemKey key)
        {
            _key = key;
        }

        public IWorklistItemKey Key
        {
            get { return _key; }
            set { _key = value; }
        }

        public override bool Equals(object obj)
        {
            WorklistItemBase that = (WorklistItemBase)obj;
            return (that != null) &&
                (this.Key == that.Key);
        }

        public override int GetHashCode()
        {
            return this.Key.GetHashCode();
        }
    }

    public abstract class WorklistBase : IWorklist
    {
        #region IWorklist Members

        public virtual IList GetWorklist(IPersistenceContext context)
        { return null; }

        public virtual int GetWorklistCount(IPersistenceContext context)
        { return -1; }

        #endregion
    }

    public abstract class WorklistBase<T> : WorklistBase where T : IPersistenceBroker
    {
        protected static T GetBroker(IPersistenceContext context)
        {
            return context.GetBroker<T>();
        }
    }
}
