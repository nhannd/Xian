#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections;
using System.Text;

using ClearCanvas.Enterprise.Hibernate.Hql;
using NHibernate;
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Enterprise.Hibernate
{
    /// <summary>
    /// Abstract base class for all NHibernate broker implementations.
    /// </summary>
    public abstract class Broker : IPersistenceBroker
    {
        private PersistenceContext _ctx;

        /// <summary>
        /// Returns the persistence context associated with this broker instance.
        /// </summary>
        protected PersistenceContext Context
        {
            get { return _ctx; }
        }

        public void SetContext(IPersistenceContext context)
        {
            _ctx = (PersistenceContext)context;
        }

        /// <summary>
        /// Executes the specified <see cref="HqlQuery"/> against the database, returning the results
        /// as an untyped <see cref="IList"/>.
        /// </summary>
        /// <param name="query">the query to execute</param>
        /// <returns>the result set</returns>
        public IList<T> ExecuteHql<T>(HqlQuery query)
        {
            return ExecuteHql<T>(query.BuildHibernateQueryObject(_ctx));
        }

        /// <summary>
        /// Executes the specified <see cref="NHibernate.IQuery"/> against the database, returning the results
        /// as an untyped <see cref="IList"/>.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public IList<T> ExecuteHql<T>(NHibernate.IQuery query)
        {
            return query.List<T>();
        }

        public T ExecuteHqlUnique<T>(HqlQuery query)
        {
            return ExecuteHqlUnique<T>(query.BuildHibernateQueryObject(_ctx));
        }

        public T ExecuteHqlUnique<T>(NHibernate.IQuery query)
        {
            return query.UniqueResult<T>();
        }

    }
}
