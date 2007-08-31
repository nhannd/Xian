using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Core
{
    /// <summary>
    /// Defines the public inteface for a Sub-Select condition.  
    /// </summary>
    /// <typeparam name="T">A class derived from <see cref="SearchCriteria"/></typeparam>
    public interface ISubSelect<T>
        where T : SearchCriteria
    {
        /// <summary>
        /// Specifies the sub query for EXISTS conditions.
        /// </summary>
        /// <param name="val"></param>
        void Exists(T val);

        /// <summary>
        /// Specifies the sub query for NOT EXISTS conditions.
        /// </summary>
        /// <param name="val"></param>
        void NotExists(T val);
    } 
}
