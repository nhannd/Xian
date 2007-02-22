using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Enterprise;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare.Workflow
{
    public interface IWorklistItem
    {
    }

    public interface IWorklistQueryResult
    {
    }

    public interface IWorklist
    {
        SearchCriteria SearchCriteria { get; set; }
        IList GetWorklist(IPersistenceContext context);
        IList GetWorklist(IPersistenceContext context, SearchCriteria additionalCriteria);
        IList GetQueryResultForWorklistItem(IPersistenceContext context, IWorklistItem item);

        //void Subscribe(string callback);   
    }

}
