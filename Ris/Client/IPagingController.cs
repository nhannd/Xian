using System;
using System.Collections.Generic;

using ClearCanvas.Enterprise;

namespace ClearCanvas.Ris.Client
{
    public delegate void QueryEventHandler();

    public interface IPagingController<T>
    {
        /// <summary>
        /// 
        /// </summary>
        int PageSize {get; set;}

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool HasNext { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool HasPrev { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IList<T> GetNext();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IList<T> GetPrev();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IList<T> GetFirst(SearchCriteria criteria);

        /// <summary>
        /// 
        /// </summary>
        event QueryEventHandler OnInitialQueryEvent;
    }
}
