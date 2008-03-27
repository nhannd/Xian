using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare.Brokers
{
    /// <summary>
    /// Defines a common base interface for worklist item brokers.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public interface IWorklistItemBroker<TItem>: IPersistenceBroker
    {
        /// <summary>
        /// Gets the set of items in the worklist.
        /// </summary>
        /// <param name="worklist"></param>
        /// <param name="wqc"></param>
        /// <returns></returns>
        IList<TItem> GetWorklistItems(Worklist worklist, IWorklistQueryContext wqc);

        /// <summary>
        /// Gets a count of the items in the worklist.
        /// </summary>
        /// <param name="worklist"></param>
        /// <param name="wqc"></param>
        /// <returns></returns>
        int CountWorklistItems(Worklist worklist, IWorklistQueryContext wqc);
    }
}
