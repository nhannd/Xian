#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Core
{
    /// <summary>
    /// Defines the public inteface for a Sub-Select condition.  
    /// </summary>
    /// <typeparam name="T">A class derived from <see cref="SearchCriteria"/></typeparam>
    public interface IRelatedEntityCondition<T>
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
