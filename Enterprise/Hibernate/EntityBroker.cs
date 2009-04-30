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
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Core.Modelling;
using NHibernate;
using NHibernate.Expression;
using ClearCanvas.Enterprise.Hibernate.Hql;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Enterprise.Hibernate
{
    /// <summary>
    /// Abstract base class for NHibernate implemenations of <see cref="IEntityBroker{TEntity, TSearchCriteria}"/>.
    /// </summary>
    /// <typeparam name="TEntity">The entity class on which this broker operates</typeparam>
    /// <typeparam name="TSearchCriteria">The corresponding <see cref="SearchCriteria"/> class.</typeparam>
    public abstract class EntityBroker<TEntity, TSearchCriteria> : Broker, IEntityBroker<TEntity, TSearchCriteria>
        where TEntity : Entity
        where TSearchCriteria : SearchCriteria, new()
    {
        public IList<TEntity> Find(TSearchCriteria criteria)
        {
            return Find(criteria, null);
        }

        public IList<TEntity> Find(TSearchCriteria[] criteria)
        {
            return Find(criteria, null);
        }

        public IList<TEntity> Find(TSearchCriteria criteria, SearchResultPage page)
        {
            return Find(new TSearchCriteria[] { criteria }, page);
        }

        public IList<TEntity> Find(TSearchCriteria[] criteria, SearchResultPage page)
        {
            return Find(criteria, page, false);
        }

        public IList<TEntity> Find(TSearchCriteria[] criteria, SearchResultPage page, bool cache)
        {
			HqlProjectionQuery query = new HqlProjectionQuery(new HqlFrom(typeof(TEntity).Name, "x"));
			query.Page = page;

			// add fetch joins
			foreach (string fetchJoin in GetDefaultFetchJoins())
        	{
        		query.Froms[0].Joins.Add(new HqlJoin("x." + fetchJoin, null, HqlJoinMode.Inner, true));
        	}

            HqlOr or = new HqlOr();
            foreach (TSearchCriteria c in criteria)
            {
                HqlAnd and = new HqlAnd(HqlCondition.FromSearchCriteria("x", c));
                if (and.Conditions.Count > 0)
                    or.Conditions.Add(and);

                query.Sorts.AddRange(HqlSort.FromSearchCriteria("x", c));
            }

            if (or.Conditions.Count > 0)
                query.Conditions.Add(or);

            query.Cacheable = cache;

            return ExecuteHql<TEntity>(query);
        }

        public IList<TEntity> FindAll()
        {
            return FindAll(true);
        }

    	public IList<TEntity> FindAll(bool includeDeactivated)
    	{
    		TSearchCriteria where = new TSearchCriteria();

			// if the entity class supports deactivation, apply this condition
			if(!includeDeactivated && AttributeUtils.HasAttribute<DeactivationFlagAttribute>(typeof(TEntity)))
			{
				string propertyName = AttributeUtils.GetAttribute<DeactivationFlagAttribute>(typeof (TEntity)).PropertyName;
				SearchCondition<bool> c = new SearchCondition<bool>(propertyName);
				c.EqualTo(false);
				where.SubCriteria[propertyName] = c;
			}

			return Find(where, null);
		}

    	public TEntity FindOne(TSearchCriteria criteria)
        {
            return FindOne(new TSearchCriteria[] { criteria });
        }

        public TEntity FindOne(TSearchCriteria[] criteria)
        {
            IList<TEntity> results = Find(criteria, new SearchResultPage(0, 1));

			if (results.Count == 0)
			{
				throw new EntityNotFoundException(null);
			}

            return results[0];
        }

        public long Count(TSearchCriteria criteria)
        {
            return Count(new TSearchCriteria[] { criteria });
        }

        public long Count(TSearchCriteria[] criteria)
        {
            HqlQuery query = new HqlQuery(string.Format("select count(*) from {0} x", typeof(TEntity).Name));

            // for a "count" query, sort conditions that may be present in the
            // criteria object must be ignored- therefore, only the conditions are added to the query
            HqlOr or = new HqlOr();
            foreach (TSearchCriteria c in criteria)
            {
                HqlAnd and = new HqlAnd(HqlCondition.FromSearchCriteria("x", c));
                if (and.Conditions.Count > 0)
                    or.Conditions.Add(and);
            }

            if (or.Conditions.Count > 0)
                query.Conditions.Add(or);

            // expect exactly one integer result
            return ExecuteHqlUnique<long>(query);
        }



        public TEntity Load(EntityRef entityRef)
        {
            return this.Context.Load<TEntity>(entityRef);
        }

        public TEntity Load(EntityRef entityRef, EntityLoadFlags flags)
        {
            return this.Context.Load<TEntity>(entityRef, flags);
        }

        public void Delete(TEntity entity)
        {
			if (this.Context.ReadOnly)
				throw new InvalidOperationException("Cannot delete entity via read-only persistence context.");

            this.Context.Session.Delete(entity);
        }

        protected void LoadAssociation(TEntity entity, object association)
        {
            if(!this.Context.Session.Contains(entity))
                throw new PersistenceException(SR.ExceptionEntityNotInContext, null);

            if (!NHibernateUtil.IsInitialized(association))
                NHibernateUtil.Initialize(association);
        }

		/// <summary>
		/// Gets the set of fetch-joins that will be placed into the query by default.
		/// </summary>
		/// <remarks>
		/// Sub-classes may override this to provide a default set of fetch-joins, where
		/// each entry in the array is the name of a property or compound property on the
		/// entity class (e.g. for a Procedure, one might have "Order" and "Order.Patient").
		/// WARNING: this API is subject to change.
		/// </remarks>
		/// <returns></returns>
		protected virtual string[] GetDefaultFetchJoins()
		{
			return new string[] { };
		}
	}
}
