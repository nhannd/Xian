using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

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
    /// Abstract base class for NHibernate implemenations of <see cref="IEntityBroker"/>.
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

        public virtual IList<TEntity> Find(TSearchCriteria criteria, SearchResultPage page)
        {
            return Find(new TSearchCriteria[] { criteria }, page);
        }

        public virtual IList<TEntity> Find(TSearchCriteria[] criteria, SearchResultPage page)
        {
            HqlQuery query = new HqlQuery(string.Format("from {0} x", typeof(TEntity).Name));
            query.Page = page;

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

            return MakeTypeSafe<TEntity>(ExecuteHql(query));
        }

        public IList<TEntity> FindAll()
        {
            return Find(new TSearchCriteria(), null);
        }

        public TEntity FindOne(TSearchCriteria criteria)
        {
            return FindOne(new TSearchCriteria[] { criteria });
        }

        public TEntity FindOne(TSearchCriteria[] criteria)
        {
            IList<TEntity> results = Find(criteria, new SearchResultPage(0, 1));

            if (results.Count == 0)
                throw new EntityNotFoundException(null);

            return results[0];
        }

        public int Count(TSearchCriteria criteria)
        {
            return Count(new TSearchCriteria[] { criteria });
        }

        public int Count(TSearchCriteria[] criteria)
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

            IList results = ExecuteHql(query);

            // expect exactly one integer result
            return (int)results[0];
        }



        public TEntity Load(EntityRef entityRef)
        {
            return (TEntity)this.Context.Load(entityRef);
        }

        public TEntity Load(EntityRef entityRef, EntityLoadFlags flags)
        {
            return (TEntity)this.Context.Load(entityRef, flags);
        }

        public virtual void Delete(TEntity entity)
        {
            if(this.Context.ReadOnly)
                 throw new Exception();  //TODO elaborate

            this.Context.Session.Delete(entity);
        }

        protected void LoadAssociation(TEntity entity, object association)
        {
            if(!this.Context.Session.Contains(entity))
                throw new PersistenceException(SR.ExceptionEntityNotInContext, null);

            if (!NHibernateUtil.IsInitialized(association))
                NHibernateUtil.Initialize(association);
        }
    }
}
