#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Diagnostics;

namespace ClearCanvas.Common.Statistics
{
    /// <summary>
    /// Helper class
    /// </summary>
    public static class StatisticsHelper
    {
        /// <summary>
        /// Resolves the ID of a statistics based on its context and name
        /// </summary>
        /// <param name="stat"></param>
        /// <returns></returns>
        public static object ResolveID(IStatistics stat)
        {
            object key = null;
            if (stat.Context != null)
            {
                key = String.Format("{0}.{1}", stat.Context.ID, stat.Name);
            }
            else
            {
                key = String.Format("{0}", stat.Name);
            }

            Debug.Assert(key != null);
            return key;
        }
    }
}