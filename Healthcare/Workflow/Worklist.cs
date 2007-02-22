using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Enterprise;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare.Workflow
{
    public class Worklist : IWorklist
    {
        private SearchCriteria _criteria;

        public virtual IList GetWorklist(IPersistenceContext context)
        { return GetWorklist(context, null); }

        public virtual IList GetWorklist(IPersistenceContext context, SearchCriteria additionalCriteria)
        { return null; }

        public virtual IList GetQueryResultForWorklistItem(IPersistenceContext context, IWorklistItem item)
        { return null; }

        public SearchCriteria SearchCriteria
        {
            get { return _criteria; }
            set { _criteria = value; }
        }
    }
}
