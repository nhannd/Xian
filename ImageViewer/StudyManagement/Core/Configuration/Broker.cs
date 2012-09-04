#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ClearCanvas.ImageViewer.StudyManagement.Core.Configuration
{
    public abstract class Broker
    {
        protected Broker(ConfigurationDataContext context)
        {
            Context = context;
        }

        protected ConfigurationDataContext Context { get; private set; }

        // taken from here: http://stackoverflow.com/questions/2910471/querying-a-single-column-with-linq
        public List<TResult> GetSingleColumn<T, TResult>(
            Expression<Func<T, bool>> predicate,
            Expression<Func<T, TResult>> select)
            where T : class
        {
            var q = Context.GetTable<T>().AsQueryable();
            if (predicate != null)
                q = q.Where(predicate).AsQueryable();
            return q.Select(select).ToList();
        }
    }
}
